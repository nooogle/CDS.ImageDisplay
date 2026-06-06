# Annotations Feature Plan

## Goal

Add an `Annotations` namespace to `CDS.ImageDisplay.WinForms` that lets users interactively draw and label shapes on a `BitmapDisplayPanel`. Primary use case: image labelling apps (bounding boxes, regions, keypoints) where the user draws a rough freehand gesture and the library suggests the most likely shape.

---

## Key Design Decisions

### 1. Decoupled annotation model

The `Annotations` namespace defines its own geometry types rather than reusing the `Overlays` shape classes. This is intentional:

- Overlay shapes are stateless drawing helpers; annotation geometry owns its own coordinates and can be hit-tested, dragged, and resized.
- The annotation layer can evolve independently (serialization, custom metadata, etc.) without coupling to the presentation-only overlay layer.
- Under the hood, annotation geometry classes use the same GDI+ primitives and `DrawingToolsPool` as the overlay layer — just through their own draw methods.

### 2. Separation of geometry and annotation metadata

- `AnnotationGeometry` (abstract) — owns the shape coordinates and appearance (`DrawingSpec`). Responsible for drawing, hit-testing, and applying drag operations.
- `Annotation` (base class) — wraps a geometry and adds `Id`, `Title`, and `Notes`. User apps subclass `Annotation` to attach custom metadata (labels, confidence scores, categories, etc.).

### 3. Shape descriptors are the extension point

`IAnnotationShapeDescriptor` serves three purposes:
1. Names the shape in the popup menu.
2. Scores how well a `FreehandPath` matches the shape (used by Auto recognition).
3. Constructs an `AnnotationGeometry` instance from a freehand path.

Registering custom descriptors is how user apps add custom shape types.

### 4. Interaction model mirrors `SingleLineSelectionManager`

- `AnnotationManager` is a `Component` attached to a `BitmapDisplayPanel` via a property setter (subscribe/unsubscribe pattern, identical to `SingleLineSelectionManager`).
- Mouse state is a private enum (`AnnotationInteractionState`) — not public.
- All primary geometry is stored in image pixel coordinates; display coordinates are derived on demand via `MapImageToDisplay` / `MapDisplayToImage`.

### 5. Two creation paths: click vs. drag

**Click (below minimum gesture threshold):**
- A click (bounding box smaller than `MinimumGestureSize` pixels on any side) immediately commits a `CrosshairAnnotationGeometry` at the click point, with no menu shown.
- The crosshair arms are sized in display pixels so they remain visually constant regardless of zoom.
- This is the fast path for marking the centre of a feature.

**Drag (above threshold) — two-phase:**
1. User holds left mouse button and draws a freehand stroke. The raw path is rendered as a thin translucent polyline in real time.
2. On mouse up, a `ContextMenuStrip` appears offset below-right of the bounding box. The **Auto** entry (shown in bold, pre-highlighted) shows the top recognized shape in parentheses: *"Auto (Rectangle)"*.
3. Selecting a menu item commits a new `Annotation`; pressing Escape (or clicking outside) cancels.

### 6. Auto recognition and polygon as catch-all

The `AnnotationShapeRecognizer` ranks all registered descriptors by `FitScore`. Auto selects the highest scorer **if its score exceeds a confidence threshold** (e.g. 0.5). If no shape clears the threshold, Auto falls back to **Polygon**, which is always present and always scores the full path as valid. Polygon is therefore both the catch-all and the explicit "I drew an irregular region" choice.

### 7. Editing uses a handle system inspired by `SingleROIManager`

Selected annotations show resize/drag handles. Hit-testing returns a typed `AnnotationHitInfo` that identifies what was clicked (handle index, move area, miss). The geometry class interprets hit info and applies drag deltas. Shape-specific geometry logic stays inside the geometry class.

### 8. Per-shape default colours

Each built-in descriptor sets a distinctive default colour so labelling apps get a usable result without configuration:

| Shape | Default pen colour |
|---|---|
| Rectangle | `Color.LimeGreen` |
| Circle | `Color.DodgerBlue` |
| Ellipse | `Color.CornflowerBlue` |
| Line | `Color.Gold` |
| Polygon | `Color.Orange` |
| Crosshair | `Color.Tomato` |

These are the pen colour defaults for the `DrawingSpec` constructed by each descriptor. The caller can override them after creation.

### 9. JSON serialization

All annotation types support `System.Text.Json` serialization out of the box:

- `AnnotationGeometry` is decorated with `[JsonPolymorphic]` + `[JsonDerivedType(...)]` for each built-in subclass, enabling round-trip serialization without additional configuration.
- `Annotation` likewise supports polymorphic serialization so user subclasses can participate (they register their own derived type attribute or use the discriminator manually).
- New JSON converters added: `PointConverter`, `PointFConverter`, `RectangleConverter`, `RectangleFConverter`. `ColorJsonConverter` already exists.
- `DrawingSpec`, `PenSpec`, `BrushSpec`, `FontSpec` gain `[JsonInclude]` on their settable properties.
- A static `AnnotationSerializer` helper class provides `Serialize` / `Deserialize` methods pre-configured with the right `JsonSerializerOptions` so callers don't need to assemble converters manually.

---

## Component Breakdown

### `FreehandPath`

Immutable value type built from a `List<PointF>` captured during the drawing gesture (image coordinates).

```
Properties:
  IReadOnlyList<PointF> Points
  RectangleF BoundingBox
  PointF Centroid
  float ApproximatePerimeter

Static factory:
  FreehandPath.From(IEnumerable<PointF> points)
```

### `AnnotationHitInfo`

Describes what the mouse is over on a selected annotation.

```
enum AnnotationHitKind { None, MoveBody, Handle }

record AnnotationHitInfo(AnnotationHitKind Kind, int HandleIndex)
  static readonly Miss = new(None, -1)
  static readonly Move = new(MoveBody, -1)
  static Handle(int index) = new(Handle, index)
```

### `AnnotationGeometry` (abstract)

Base class for all shapes. Lives in `CDS.ImageDisplay.Annotations`.

```
Properties:
  DrawingSpec Drawing          // appearance (pen, brush, font)

Abstract methods:
  RectangleF GetBoundingBox()  // image coordinates; RectangleF.Empty for point shapes
  void Draw(BitmapDisplayPanel panel, Graphics g, bool isSelected)
  AnnotationHitInfo HitTest(BitmapDisplayPanel panel, Point displayPoint, int hitBorder)
  void ApplyDrag(BitmapDisplayPanel panel, AnnotationHitInfo hit, Point dragDeltaDisplay)
  AnnotationGeometry Clone()
```

Drawing when `isSelected` should include handle squares/circles at drag points.

### Built-in geometry classes

| Class | Stores | Handles |
|---|---|---|
| `RectAnnotationGeometry` | `Rectangle` (image px) | 8 grapple corners + edges, body move |
| `CircleAnnotationGeometry` | `Point` centre, `int` radius (image px) | 4 cardinal edge handles, body move |
| `EllipseAnnotationGeometry` | `Rectangle` bounding box (image px) | 8 grapple corners + edges, body move |
| `LineAnnotationGeometry` | `Point` start, `Point` end (image px) | 2 endpoint handles, whole-line move |
| `PolygonAnnotationGeometry` | `IReadOnlyList<Point>` vertices (image px) | Per-vertex handles, body move |
| `CrosshairAnnotationGeometry` | `Point` centre (image px), `float Length`, `float CentreGap` (display px) | Body move only (no resize) |

Note: `CrosshairAnnotationGeometry` arms are sized in **display** pixels so they remain constant-size at all zoom levels, matching the behaviour of the existing `CrosshairShape` overlay.

### `Annotation` (base class)

```
Properties:
  Guid Id                    // set at construction, immutable
  string Title               // "" by default
  string Notes               // "" by default
  AnnotationGeometry Geometry

Constructor:
  Annotation(AnnotationGeometry geometry)

Virtual methods:
  virtual Annotation Clone() // clones geometry + copies Title/Notes/Id
```

User apps subclass `Annotation` to add domain metadata.

### `IAnnotationShapeDescriptor`

```
interface IAnnotationShapeDescriptor
  string Name                // shown in popup menu
  string Description         // tooltip
  float FitScore(FreehandPath path)   // 0..1; 0 = impossible, 1 = perfect match
  AnnotationGeometry CreateGeometry(FreehandPath path)
```

### `AnnotationShapeRecognizer`

Static utility.

```
static class AnnotationShapeRecognizer
  static IReadOnlyList<(IAnnotationShapeDescriptor, float confidence)>
      Rank(FreehandPath path, IEnumerable<IAnnotationShapeDescriptor> descriptors)
```

Built-in recognition heuristics (each returns a 0..1 score):

- **Rectangle**: bounding-box fill ratio > 0.6 AND convex hull closely matches a rectangle.
- **Circle**: aspect ratio of bounding box near 1.0 AND mean-radius variance below threshold.
- **Ellipse**: aspect ratio away from 1.0 AND hull closely follows ellipse equation.
- **Line**: bounding box is narrow/elongated AND mean perpendicular deviation from the line through endpoints is small.
- **Polygon**: always returns 0.3 (low baseline confidence — wins only when no other shape clears the confidence threshold of 0.5).

The **Auto** selection is the top-ranked shape if its score ≥ 0.5, otherwise Polygon.

The `CrosshairDescriptor` does not participate in `Rank` (it is never suggested for a drag gesture). It is shown in the menu for completeness but the auto path never picks it from a drag.

### `AnnotationShapeMenu`

Internal helper that creates and shows a `ContextMenuStrip`. Not part of the public API.

```
internal sealed class AnnotationShapeMenu
  void Show(Control parent, Point screenPoint,
            IReadOnlyList<IAnnotationShapeDescriptor> rankedDescriptors,
            Action<IAnnotationShapeDescriptor?> callback)
```

The first item is always "Auto (*ShapeName*)" in bold. Remaining items are the ranked shapes, then a separator, then Crosshair. Escape and clicking outside invoke `callback(null)`.

### `AnnotationSerializer`

```
static class AnnotationSerializer
  static string Serialize(IEnumerable<Annotation> annotations)
  static IReadOnlyList<Annotation> Deserialize(string json)
  static IReadOnlyList<T> Deserialize<T>(string json) where T : Annotation
  static JsonSerializerOptions CreateOptions()   // for callers who need the options directly
```

### `AnnotationManager`

The public `Component` that integrates everything.

```
public partial class AnnotationManager : Component

Properties:
  BitmapDisplayPanel? BitmapDisplayPanel    // subscribe/unsubscribe pattern
  IReadOnlyList<Annotation> Annotations     // current annotation list
  Annotation? SelectedAnnotation            // null = nothing selected
  bool CanCreate                            // default true
  bool CanEdit                              // default true
  int DragBorder                            // hit-test tolerance in display pixels, default 8
  int MinimumGestureSize                    // bounding-box threshold for click vs. drag, default 5 (display px)

Methods:
  void AddAnnotation(Annotation annotation)
  void RemoveAnnotation(Annotation annotation)
  void ClearAnnotations()
  void RegisterShapeDescriptor(IAnnotationShapeDescriptor descriptor)

Events:
  EventHandler<AnnotationCreatedEventArgs>    AnnotationCreated
  EventHandler<AnnotationModifiedEventArgs>   AnnotationModified
  EventHandler<AnnotationDeletedEventArgs>    AnnotationDeleted
  EventHandler<AnnotationSelectedEventArgs>   AnnotationSelected
  EventHandler                                AnnotationDeselected
```

Note: `DefaultDrawingSpec` is removed — each descriptor provides shape-specific default colours via `CreateGeometry`.

#### Internal state machine

```
enum AnnotationInteractionState
{
    Idle,
    Drawing,       // left button held, freehand path accumulating
    MenuOpen,      // mouse up (drag gesture), waiting for shape selection
    Selected,      // an annotation is selected, showing handles
    Dragging,      // dragging selected annotation or a handle
}
```

State transitions:

```
Idle  ──────── MouseDown on empty space ──────────────►  Drawing
Idle  ──────── MouseDown on annotation ───────────────►  Selected (+ fires AnnotationSelected)

Drawing  ─────  MouseMove ────────────────────────────►  Drawing (append point, invalidate)
Drawing  ─────  MouseUp, bbox < MinimumGestureSize ───►  Idle (commit Crosshair, fire AnnotationCreated)
Drawing  ─────  MouseUp, bbox ≥ MinimumGestureSize ───►  MenuOpen (show shape menu)
Drawing  ─────  Escape ───────────────────────────────►  Idle (discard path)

MenuOpen  ────  shape chosen ─────────────────────────►  Idle (create annotation, fire AnnotationCreated)
MenuOpen  ────  Escape / cancel ──────────────────────►  Idle (discard path)

Selected  ────  MouseDown on handle/body ─────────────►  Dragging
Selected  ────  MouseDown on empty space ─────────────►  Idle (deselect, fire AnnotationDeselected)
Selected  ────  Delete key ───────────────────────────►  Idle (remove annotation, fire AnnotationDeleted)
Selected  ────  Escape ───────────────────────────────►  Idle (deselect)

Dragging  ────  MouseMove ────────────────────────────►  Dragging (update geometry live)
Dragging  ────  MouseUp ──────────────────────────────►  Selected (commit drag, fire AnnotationModified)
Dragging  ────  Escape ───────────────────────────────►  Selected (revert to pre-drag geometry)
```

#### Mouse cursor behaviour

| State / hover | Cursor |
|---|---|
| Idle, hovering over annotation | `SizeAll` |
| Idle, hovering over annotation handle | shape-specific (see below) |
| Drawing | `Cross` |
| Dragging body | `SizeAll` |
| Dragging corner handle | `SizeNWSE` / `SizeNESW` (alternating corners) |
| Dragging edge handle | `SizeNS` / `SizeWE` |
| Dragging endpoint (line) or vertex (polygon) | `Cross` |
| Dragging crosshair centre | `SizeAll` |

---

## Event Args Classes

```csharp
sealed class AnnotationCreatedEventArgs(Annotation annotation) : EventArgs
sealed class AnnotationModifiedEventArgs(Annotation annotation) : EventArgs
sealed class AnnotationDeletedEventArgs(Annotation annotation) : EventArgs
sealed class AnnotationSelectedEventArgs(Annotation annotation) : EventArgs
```

---

## File / Namespace Layout

```
CDS.ImageDisplay.WinForms/
  Annotations/
    AnnotationGeometry.cs
    Annotation.cs
    FreehandPath.cs
    AnnotationHitInfo.cs
    IAnnotationShapeDescriptor.cs
    AnnotationShapeRecognizer.cs
    AnnotationSerializer.cs
    AnnotationInteractionState.cs      (internal enum)
    AnnotationManager.cs
    AnnotationManager.Designer.cs
    Delegates.cs                       (event arg types)
    Shapes/
      RectAnnotationGeometry.cs
      CircleAnnotationGeometry.cs
      EllipseAnnotationGeometry.cs
      LineAnnotationGeometry.cs
      PolygonAnnotationGeometry.cs
      CrosshairAnnotationGeometry.cs
      RectAnnotationDescriptor.cs
      CircleAnnotationDescriptor.cs
      EllipseAnnotationDescriptor.cs
      LineAnnotationDescriptor.cs
      PolygonAnnotationDescriptor.cs
      CrosshairAnnotationDescriptor.cs
    Internal/
      AnnotationShapeMenu.cs
      FreehandPathOverlay.cs           (draws the raw path during Drawing state)
    Json/
      PointConverter.cs
      PointFConverter.cs
      RectangleConverter.cs
      RectangleFConverter.cs
```

---

## Implementation Phases

### Phase 1 — Foundation types
- `FreehandPath`, `AnnotationHitInfo`, `AnnotationGeometry` (abstract), `Annotation` (base), `IAnnotationShapeDescriptor`
- No UI yet; covers the model layer. Allows unit tests for geometry in isolation.

### Phase 2 — Built-in shapes (all six)
- All six geometry + descriptor classes.
- Each geometry class: `Draw()` (not selected), `GetBoundingBox()`, `Clone()`.
- Descriptors: `FitScore()` (stub returning 0 except Polygon = 0.3) + `CreateGeometry()` with default colours.
- Unit tests for bounding-box math and draw-rect computation per shape.

### Phase 3 — Shape recognition
- `AnnotationShapeRecognizer.Rank()`.
- Implement `FitScore()` heuristics in each descriptor.
- Unit tests with synthetic path data for each shape type.

### Phase 4 — Drawing during interaction
- `FreehandPathOverlay` paints the live path in Drawing state.
- Handle rendering in each geometry class when `isSelected = true`.
- Selected annotation handles drawn in `AnnotationManager.BitmapDisplayPanel_OnPaintOver`.

### Phase 5 — AnnotationManager core
- State machine, freehand capture, `MinimumGestureSize` click detection.
- Click path → immediate Crosshair commit.
- Drag path → `AnnotationShapeMenu` → commit on selection.
- `AnnotationCreated` event.
- `CanCreate` guard.

### Phase 6 — Selection and editing
- Hit-testing existing annotations (topmost first; smallest area to break ties).
- `ApplyDrag()` in each geometry class.
- Pre-drag snapshot for Escape-to-revert.
- `CanEdit` guard.
- `AnnotationSelected`, `AnnotationModified`, `AnnotationDeselected` events.
- Cursor management.

### Phase 7 — Keyboard
- Delete → `AnnotationDeleted`.
- Escape → cancel current operation / deselect / revert drag.
- Arrow keys nudge selected annotation (1 px image; Ctrl = 10 px).

### Phase 8 — JSON serialization
- JSON converters for `Point`, `PointF`, `Rectangle`, `RectangleF` in `Annotations/Json/`.
- `[JsonPolymorphic]` / `[JsonDerivedType]` on `AnnotationGeometry` and `Annotation`.
- `[JsonInclude]` on `DrawingSpec`, `PenSpec`, `BrushSpec`, `FontSpec` properties.
- `AnnotationSerializer` helper with pre-configured options.
- Round-trip unit tests for each geometry type.

### Phase 9 — Demo integration
- Create `DemoForms/AnnotationsDemo/` folder in `CDS.ImageDisplay.Demo`.
- **`FormAnnotationsSimple`** — minimal viable demo:
  - Image in `FitToWindow` mode with an `AnnotationManager` wired to the panel using all defaults.
  - Status label at the bottom showing annotation count and the title/shape-type of the most recently created annotation.
  - Wire `AnnotationCreated` to update the status label.
- **`FormAnnotationsDetailed`** — full labelling-app demo:
  - Left: `BitmapDisplayPanel` + `AnnotationManager`.
  - Right docked panel: `ListView` (columns: Type, Title) listing all annotations; `TextBox` for Title and multi-line `TextBox` for Notes, populated on `AnnotationSelected` and saved on `TextChanged` + `AnnotationModified`.
  - Buttons: **Clear all** (`ClearAnnotations`), **Save JSON** (file-save dialog → `AnnotationSerializer.Serialize`), **Load JSON** (file-open dialog → `AnnotationSerializer.Deserialize`, replacing current annotations).
  - Status bar wired to all five events: `AnnotationCreated`, `AnnotationModified`, `AnnotationDeleted`, `AnnotationSelected`, `AnnotationDeselected`.
- Register both forms in `FormTestLauncher.AddAnnotationsDemoNodes()` under an **"Annotations"** group, following the same pattern as `AddROIDemoNodes()` and `AddLineSelectionDemoNodes()`.
  - Simple entry: name = `"Annotations: simple"`, tooltip = `"Basic demo of the annotation manager — draw shapes on an image by clicking or dragging"`.
  - Detailed entry: name = `"Annotations: detailed"`, tooltip = `"Full labelling-app demo with annotation list, title/notes editing, and JSON save/load"`.

---

## Demo Forms

The existing demo project (`CDS.ImageDisplay.Demo`) uses a launcher form with a tree menu organized into groups. Annotations gets its own group registered in `FormTestLauncher.AddDemos()` via a new `AddAnnotationsDemoNodes()` method, following the same pattern as ROI and line selection.

### New folder: `DemoForms/AnnotationsDemo/`

#### `FormAnnotationsSimple`
The minimum viable demo — mirrors `FormROISelectionSimple` and `FormLineSelectionSimple`.

- An image in `FitToWindow` mode.
- An `AnnotationManager` wired to the panel with all defaults.
- A status label at the bottom showing: annotation count, and the title/shape-type of the most recently created annotation.
- No editing of Title/Notes — just shows the feature working.

**Launcher entry:** name = `"Annotations: simple"`, tooltip = `"Basic demo of the annotation manager — draw shapes on an image by clicking or dragging"`.

#### `FormAnnotationsDetailed`
A fuller labelling-app demo — mirrors `FormROISelectionDetailed` and `FormLineSelectionDetailed`.

- Left: `BitmapDisplayPanel` with `AnnotationManager`.
- Right panel (docked):
  - `ListView` listing all current annotations (columns: Type, Title).
  - `TextBox` for Title and multi-line `TextBox` for Notes, updated when an annotation is selected.
  - Buttons: **Clear all**, **Save JSON** (saves to a user-chosen file), **Load JSON** (loads from a user-chosen file, replacing current annotations).
- Status bar showing selected annotation info.
- Demonstrates `AnnotationCreated`, `AnnotationModified`, `AnnotationDeleted`, `AnnotationSelected`, `AnnotationDeselected` events all wired up.
- Demonstrates `AnnotationSerializer.Serialize` / `Deserialize`.

**Launcher entry:** name = `"Annotations: detailed"`, tooltip = `"Full labelling-app demo with annotation list, title/notes editing, and JSON save/load"`.

---

## Unit Tests

Tests live in `UnitTests/Annotations/`. Each test class follows `MethodName_Scenario_ExpectedResult` naming and uses AwesomeAssertions.

### `FreehandPathTests`

| Test | What it covers |
|---|---|
| `From_EmptySequence_ReturnsBoundingBoxEmpty` | Guard against zero-point paths |
| `From_SinglePoint_BoundingBoxIsZeroSized` | Edge case: click-level gesture |
| `From_HorizontalLine_BoundingBoxHasZeroHeight` | Degenerate bounding box |
| `BoundingBox_DiagonalPath_MatchesExtents` | Normal case |
| `Centroid_SymmetricPoints_ReturnsGeometricCentre` | Centroid math |
| `ApproximatePerimeter_SingleSegment_EqualsLength` | Perimeter calculation |

### `AnnotationTests`

| Test | What it covers |
|---|---|
| `Constructor_AssignsNewGuid_IdIsNotEmpty` | Id is always set |
| `Constructor_DefaultsEmptyTitleAndNotes` | Default metadata |
| `Clone_ReturnsDifferentInstance_WithSameIdTitleNotes` | Clone identity |
| `Clone_GeometryIsIndependent_MutatingCloneDoesNotAffectOriginal` | Deep copy of geometry |

### `RectAnnotationGeometryTests`

| Test | What it covers |
|---|---|
| `GetBoundingBox_ReturnsRectangle_AsRectangleF` | Bounding box passthrough |
| `Clone_ReturnsDifferentInstance_WithSameRectangle` | Deep clone |
| `ApplyDrag_MoveBody_ShiftsRectangle` | Whole-shape move |
| `ApplyDrag_TopLeftHandle_ResizesCorrectly` | Corner resize |
| `ApplyDrag_TopEdgeHandle_ResizesHeightOnly` | Edge resize |
| `HitTest_MouseInsideBody_ReturnsMoveBody` | Body hit |
| `HitTest_MouseNearCorner_ReturnsHandle` | Handle hit |
| `HitTest_MouseOutside_ReturnsMiss` | Miss |

*(Analogous test classes for Circle, Ellipse, Line, Polygon, Crosshair geometry — covering their specific bounding-box math, clone, and hit-test logic.)*

### `CircleAnnotationGeometryTests`

| Test | What it covers |
|---|---|
| `GetBoundingBox_ReturnsSquareAroundCentreAndRadius` | Circle → square bbox |
| `ApplyDrag_EdgeHandle_ResizesRadius` | Radius resize |
| `ApplyDrag_MoveBody_ShiftsCentre` | Move |
| `HitTest_MouseOnCircumference_WithinBorder_ReturnsHandle` | Circumference hit |

### `CrosshairAnnotationGeometryTests`

| Test | What it covers |
|---|---|
| `GetBoundingBox_ReturnsEmpty` | Point shape has no image bounding box |
| `Clone_ReturnsDifferentInstance_WithSameCentre` | Clone |
| `ApplyDrag_MoveBody_ShiftsCentre` | Move |
| `HitTest_MouseAtCentre_ReturnsMoveBody` | Centre hit |
| `HitTest_MouseFar_ReturnsMiss` | Miss |

### `AnnotationShapeRecognizerTests`

| Test | What it covers |
|---|---|
| `Rank_RectangularPath_RectTopRanked` | Rectangle recognition |
| `Rank_CircularPath_CircleTopRanked` | Circle recognition |
| `Rank_ElongatedPath_LineTopRanked` | Line recognition |
| `Rank_IrregularPath_PolygonWins_WhenNothingClears Threshold` | Polygon fallback |
| `Rank_AllDescriptors_PolygonAlwaysPresent` | Polygon always in results |
| `Rank_EmptyDescriptors_ReturnsEmpty` | Guard |

### `AnnotationSerializerTests`

| Test | What it covers |
|---|---|
| `Serialize_RectAnnotation_RoundTrips` | Rect JSON round-trip |
| `Serialize_CircleAnnotation_RoundTrips` | Circle JSON round-trip |
| `Serialize_EllipseAnnotation_RoundTrips` | Ellipse JSON round-trip |
| `Serialize_LineAnnotation_RoundTrips` | Line JSON round-trip |
| `Serialize_PolygonAnnotation_RoundTrips` | Polygon JSON round-trip |
| `Serialize_CrosshairAnnotation_RoundTrips` | Crosshair JSON round-trip |
| `Serialize_MixedList_PreservesOrder` | Heterogeneous list |
| `Serialize_TitleAndNotes_RoundTrip` | Metadata preserved |
| `Serialize_DrawingSpecColour_RoundTrips` | Colour round-trip (via existing `ColorJsonConverter`) |
| `Deserialize_UnknownDiscriminator_Throws` | Robustness |

---

## Open Questions

1. **Popup menu position** — Offset below-right of the bounding box seems safest. Exact offset TBD (suggest 8 px gap from the bounding box edge).

2. **Multi-select** — Out of scope for now. Multi-select drag/delete adds significant complexity.

3. **Z-ordering** — Annotations are drawn in list order (last added = on top). No bring-to-front/send-to-back API in initial scope.

4. **Preview on menu hover** — Showing a ghost shape while hovering over a menu item would be a nice touch. Deferred.

5. **Polygon vertex editing** — Initial scope: create polygon from convex hull of freehand path, move whole shape. Full per-vertex drag deferred to a later phase (it requires a different handle model than the other shapes).
