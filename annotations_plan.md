# Annotations upgrade plan

---

## Status

| # | Item | State |
|---|------|-------|
| 1 | `Annotation.Label` property | done |
| 2 | `DragStarting` event on `AnnotationManager` | done |
| 3 | Label text rendered on annotations | done |
| 4 | Rotated rectangle overlay shape | done |
| 5 | Rotated rectangle annotation | done |
| 6 | Ellipse annotation upgraded to include angle | done |
| 7 | Line annotation arrow caps | done |

---

## Codebase orientation

**Solution**: `CDS.ImageDisplay.sln` / `.slnx` at repo root. Target `net10.0-windows`.

**Key projects**:
- `CDS.ImageDisplay.WinForms` — the library (NuGet deliverable). All annotation work goes here.
- `CDS.ImageDisplay.Demo` — WinForms demo app. Update demo forms to exercise new features.
- `UnitTests` — MSTest + AwesomeAssertions (not FluentAssertions).

**Annotation namespace** (`CDS.ImageDisplay.Annotations`):
- `Annotation.cs` — data model: `Id`, `Title`, `Notes`, `Geometry`. Base class; subclass to add domain metadata.
- `AnnotationGeometry.cs` — abstract base for all geometry types. Has `[JsonDerivedType]` attributes for the polymorphic JSON discriminator. **Every new geometry type must be registered here.**
- `AnnotationManager.cs` — the `Component` that wires mouse/keyboard/paint events on a `BitmapDisplayPanel`. Owns the `List<Annotation>`, selection state, and freehand recognition pipeline.
- `AnnotationSerializer.cs` — static JSON helpers using `System.Text.Json`. `CreateOptions()` registers the custom GDI+ type converters.
- `Delegates.cs` — event arg types (`AnnotationCreatedEventArgs`, etc.).
- `Annotations/Shapes/` — one file per geometry: `RectAnnotationGeometry`, `CircleAnnotationGeometry`, `EllipseAnnotationGeometry`, `LineAnnotationGeometry`, `PolygonAnnotationGeometry`, `CrosshairAnnotationGeometry`. Each has a paired `*Descriptor` implementing `IAnnotationShapeDescriptor`.
- `Annotations/Internal/` — `AnnotationHandleHelper` (handle drawing + hit-test math), `FreehandPathOverlay`, `FreehandPathAnalyser` (scoring helpers used by descriptors), `AnnotationShapeMenu`.

**Overlays namespace** (`CDS.ImageDisplay.Overlays`):
- `DrawingSpec.cs` — contains `PenSpec`, `BrushSpec`, `FontSpec` as nested properties. All geometry types carry one via `AnnotationGeometry.Drawing`.
- `PenSpec.cs` — already has `StartCap` / `EndCap` (`LineCap`) and `DashStyle`. `Create()` builds the GDI+ `Pen`.
- `DrawingToolsPool.cs` — caches `Pen`/`Brush`/`Font` objects; call `GetPen(spec)` / `GetBrush(spec)` in `Draw` methods.
- Overlay shapes (`RectangleShape`, `CircleShape`, `LineShape`, `EllipseShape`, `TextShape`, `CrosshairShape`, `PolygonShape`) — for static overlays, not interactive annotations.

**Demo annotation forms**: `CDS.ImageDisplay.Demo/DemoForms/AnnotationsDemo/` — `FormAnnotationsSimple.cs` and `FormAnnotationsDetailed.cs`.

**Demo overlays form**: `CDS.ImageDisplay.Demo/DemoForms/OverlaysDemo/FormOverlays.cs` — references `OverlayShapes.cs` and `OverlayPainter.cs`. Item 4 (rotated rect overlay) modifies `OverlayShapes.cs`.

**Patterns to follow**:
- New geometry type: add file in `Annotations/Shapes/`, register `[JsonDerivedType]` on `AnnotationGeometry`, register descriptor in `AnnotationManager.RegisterBuiltInDescriptors`.
- New overlay shape: add file in `Overlays/`, follow `RectangleShape` as a template (takes `BitmapDisplayPanel` + `Graphics` in `Draw`, calls `DrawingToolsPool`).
- Coordinate spaces: image coords (source bitmap pixels) ↔ display coords (control client pixels). Use `panel.MapImageToDisplay(...)` and `panel.MapDisplayToImage(...)`. Always be explicit about which space a point lives in.
- Use `DisplayPixelAlign.TopLeft` for rectangles, `DisplayPixelAlign.Centre` for points/endpoints.
- XML-doc all public members. One public type per file, filename matches type.

---

## Context

This library provides annotation primitives: data model, interactive editing, and rendering. A ground truth image labeling app (or any other consumer) is built on top of it. The library is pre-1.0 with no external clients, so breaking changes are fine.

---

## 1. `Annotation.Label` property

Add a `Label` string property to `Annotation` (default `string.Empty`). This is the machine-readable class name (e.g. `"car"`, `"pedestrian"`) that a consuming app uses for ground truth export or any other classification purpose. The existing `Title` and `Notes` properties stay for free-form display text.

Serialize to JSON like any other property. Without this, a consuming app cannot produce YOLO / COCO / Pascal VOC output from the library's model.

---

## 2. `DragStarting` event on `AnnotationManager`

Add an `AnnotationDragStartingEventArgs` carrying a clone of the annotation's geometry at the moment a drag begins. Fire `DragStarting` in `HandleMouseDownSelected`, just before `_preDragSnapshot` is stored and `_state` transitions to `Dragging`.

This is the only hook an app needs to implement undo/redo externally: snapshot on `DragStarting`, reverse on `AnnotationModified`. Combined with the existing `AnnotationCreated` and `AnnotationDeleted` events, an app has complete coverage for all mutations without the library owning an undo stack.

---

## 3. Label text rendered on annotations

Add `ShowLabels` bool to `AnnotationManager` (default `true`). When `true`, the paint pass in `BitmapDisplayPanel_OnPaintOver` draws the annotation's `Label` (falling back to `Title` if `Label` is empty) as a small tag near the top-left of the shape's bounding box, after drawing the shape itself.

Use a sensible built-in default: small white text on a semi-transparent dark backing rectangle, so the tag is readable regardless of image content. `AnnotationGeometry.Draw` does not receive the parent `Annotation`, so this text pass lives in `AnnotationManager` rather than inside `Draw`.

---

## 4. Rotated rectangle overlay shape

Add `RotatedRectangleShape` to the `Overlays` namespace alongside `RectangleShape`. Geometry: `Centre` (PointF, image coords), `Width` (float), `Height` (float), `Angle` (float, degrees clockwise) — same convention as OpenCV `RotatedRect`.

Update the overlays demo: change `Rectangle2` to `RotatedRectangle1` and increment the angle slowly during the paint timer to demonstrate rotation animation.

---

## 5. Rotated rectangle annotation

Add `RotatedRectAnnotationGeometry` to `Annotations/Shapes/`. Fields: `Center` (PointF, image coords), `Width` (float), `Height` (float), `AngleDegrees` (float). JSON discriminator: `"rotated-rect"`. Register in `AnnotationGeometry`'s `[JsonDerivedType]` list.

Add `RotatedRectAnnotationDescriptor`. `CreateGeometry` fits a minimum-area bounding rectangle to the freehand path points (rotating-calipers / PCA). Register in `AnnotationManager.RegisterBuiltInDescriptors`.

Handles: four corners plus a rotation handle (small circle above the top-centre of the rotated rect). Corner handles scale width/height; the rotation handle rotates about the centre. `ApplyImageDelta` dispatches on which handle is active.

`GetBoundingBox()` returns the axis-aligned envelope of the four rotated corners.

---

## 6. Ellipse annotation upgraded to include angle

Replace `EllipseAnnotationGeometry` (currently axis-aligned, `Rectangle Bounds`) with a rotated version. New fields: `Center` (PointF), `SemiMajor` (float), `SemiMinor` (float), `AngleDegrees` (float). JSON discriminator stays `"ellipse"`. Update the `[JsonDerivedType]` registration accordingly.

Update `EllipseAnnotationDescriptor.CreateGeometry` to fit using PCA on the freehand path points: the two eigenvectors give the major/minor axes and rotation angle, producing the new geometry. Update the descriptor's `Description` string.

Handles: endpoints of the major and minor axes (four handles), plus the centre for body-move. Dragging a major-axis endpoint scales `SemiMajor`; dragging a minor-axis endpoint scales `SemiMinor`. A rotation handle beyond one of the major-axis tips adjusts `AngleDegrees`.

`Draw` renders via `Graphics.TranslateTransform` / `RotateTransform` / `DrawEllipse`. `GetBoundingBox()` returns the axis-aligned envelope of the rotated ellipse.

---

## 7. Line annotation arrow caps

`PenSpec` already has `StartCap` and `EndCap` (`LineCap`), and `DrawingToolsPool` already applies them when creating the pen. The only change needed is in `LineAnnotationDescriptor.CreateGeometry`: set `EndCap = LineCap.ArrowAnchor` as the default. `StartCap` stays `LineCap.Flat`.

No changes to `LineAnnotationGeometry`, `PenSpec`, or `DrawingToolsPool`.

---

## What the consuming app provides

- **Class list and colors**: maintaining the set of valid labels; setting `annotation.Geometry.Drawing.Lines.Color` in response to `AnnotationCreated` / `AnnotationSelected` based on the label.
- **Undo/redo**: using `DragStarting`, `AnnotationCreated`, `AnnotationModified`, and `AnnotationDeleted` events to maintain a history stack.
- **File I/O**: `AnnotationSerializer` for the library's JSON format; conversion to YOLO / COCO / Pascal VOC as needed.
- **Image navigation**: previous/next image, unannotated filtering, etc.
- **Label assignment UI**: a class picker shown on `AnnotationCreated` or `AnnotationSelected`.
