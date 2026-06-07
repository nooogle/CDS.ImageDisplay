# Annotations

`AnnotationManager` is a WinForms component that adds interactive, freehand-created annotations to a `BitmapDisplayPanel`. Users draw shapes with the mouse; the manager recognises the gesture, shows a shape-selection menu, and commits the result. Annotations can be selected, moved, resized, deleted, styled, and serialised to JSON.

## Setup

Drop `AnnotationManager` onto a form alongside `BitmapDisplayPanel` (or create it in code), then wire up the panel:

```csharp
annotationManager.BitmapDisplayPanel = bitmapDisplayPanel;
```

That's all that's needed for interactive annotation. The manager subscribes to the panel's mouse, keyboard, and paint events automatically; assigning `null` detaches it cleanly.

## Creating annotations

| Gesture | Result |
|---|---|
| Left-click on empty image area | Crosshair at that point |
| Left-drag on empty image area | Freehand trace → shape-recognition menu |

The shape menu ranks candidates by how well the trace matches each shape and presents them in order. The user picks one or dismisses the menu (which discards the gesture). Press **Escape** during a drag to cancel without creating an annotation.

## Interacting with existing annotations

| Action | Effect |
|---|---|
| Click on an annotation | Select it |
| Click on empty area (while selected) | Deselect |
| Drag an annotation body | Move it |
| Drag a corner or edge handle | Resize it |
| Drag the rotation handle (rotated rect only) | Rotate it |
| **Delete** or **Backspace** (while selected) | Delete the annotation |
| Arrow keys (while selected) | Nudge by 1 px; **Ctrl** + arrow keys nudge by 10 px |
| **Escape** during drag | Revert to the pre-drag position |
| **Escape** while selected (no drag) | Deselect |
| **Spacebar** + drag | Pan the image (spacebar suppresses annotation interaction) |

## Built-in shape types

| Shape | Geometry class | Key properties |
|---|---|---|
| Rectangle | `RectAnnotationGeometry` | `Bounds` (`Rectangle`) |
| Rotated rectangle | `RotatedRectAnnotationGeometry` | `Center`, `Width`, `Height`, `AngleDegrees` |
| Circle | `CircleAnnotationGeometry` | `Centre`, `Radius` |
| Ellipse | `EllipseAnnotationGeometry` | `Bounds` (`Rectangle`) |
| Line | `LineAnnotationGeometry` | `Start`, `End` |
| Polygon | `PolygonAnnotationGeometry` | `Vertices` (`IReadOnlyList<Point>`) |
| Crosshair | `CrosshairAnnotationGeometry` | `Centre`, `Length`, `CentreGap` |

All geometry coordinates are in **image pixels**, independent of the current zoom and pan. `RotatedRectAnnotationGeometry` follows the OpenCV `RotatedRect` convention: `AngleDegrees` is clockwise rotation. The crosshair's `Length` and `CentreGap` are in **display pixels** so it stays constant-size at all zoom levels.

## Annotation metadata

Each `Annotation` carries:

| Property | Type | Purpose |
|---|---|---|
| `Id` | `Guid` | Stable identity; assigned automatically and preserved by `Clone()` |
| `Title` | `string` | Free-form display name, shown as a label near the shape |
| `Notes` | `string` | Free-form notes |
| `Label` | `string` | Machine-readable class name for export (e.g. `"car"`, `"defect"`) |
| `Geometry` | `AnnotationGeometry` | The shape and its visual style |

## Styling annotations

Each geometry object has a `Drawing` property of type `DrawingSpec` that controls how the shape is painted. Set it after creation to override the defaults applied by the shape descriptor:

```csharp
annotationManager.AnnotationCreated += (s, e) =>
{
    var drawing = e.Annotation.Geometry.Drawing;
    drawing.Lines.Color = Color.Cyan;
    drawing.Lines.Width = 2f;
    drawing.Fill.Color  = Color.FromArgb(30, Color.Cyan);
};
```

You can also set styles before adding annotations programmatically:

```csharp
var geometry = new RectAnnotationGeometry(new Rectangle(50, 50, 200, 100));
geometry.Drawing.Lines.Color = Color.OrangeRed;
geometry.Drawing.Lines.Width = 3f;
geometry.Drawing.Fill.Color  = Color.FromArgb(40, Color.OrangeRed);

annotationManager.AddAnnotation(new Annotation(geometry) { Label = "defect" });
```

`DrawingSpec` contains:

| Property | Type | Purpose |
|---|---|---|
| `Lines` | `PenSpec` | Outline colour, width, dash style |
| `Fill` | `BrushSpec` | Fill colour (alpha supported) |
| `Font` | `FontSpec` | Font for text rendering (used by future text shapes) |
| `Visible` | `bool` | Show/hide the shape without removing it |
| `MappingMode` | `MappingMode` | Always `ImageToDisplay` for annotations |

## Labels

When `ShowLabels` is `true` (the default), the manager draws a small tag near the top-left of each annotation's bounding box. The tag shows `Label` if set, otherwise `Title`. Set `ShowLabels = false` to suppress all labels.

## Programmatic API

```csharp
// Add an annotation with known geometry
annotationManager.AddAnnotation(new Annotation(geometry));

// Remove one
annotationManager.RemoveAnnotation(annotation);

// Remove all (does not fire individual AnnotationDeleted events)
annotationManager.ClearAnnotations();

// Read the current list
foreach (var ann in annotationManager.Annotations) { ... }

// Read or act on the selection
var selected = annotationManager.SelectedAnnotation;
```

## Events

| Event | Fires when | Args |
|---|---|---|
| `AnnotationCreated` | A gesture is committed | `AnnotationCreatedEventArgs.Annotation` |
| `AnnotationModified` | Drag or arrow-key moves/resizes a shape | `AnnotationModifiedEventArgs.Annotation` |
| `AnnotationDeleted` | An annotation is removed | `AnnotationDeletedEventArgs.Annotation` |
| `AnnotationSelected` | An annotation is clicked | `AnnotationSelectedEventArgs.Annotation` |
| `AnnotationDeselected` | The selection is cleared | — |
| `DragStarting` | A drag is about to begin | `AnnotationDragStartingEventArgs` — carries a geometry snapshot for undo |

`DragStarting` gives you a pre-drag geometry clone that is useful as an undo checkpoint:

```csharp
annotationManager.DragStarting += (s, e) =>
{
    undoStack.Push((e.Annotation, e.GeometrySnapshot));
};
```

## Controlling interactions

```csharp
annotationManager.CanCreate = false;  // prevent new annotations
annotationManager.CanEdit   = false;  // prevent move/resize/delete
```

Both properties are also settable from the WinForms designer under the **CDS** category.

`DragBorder` (default 8 px) sets the hit-test tolerance around shape edges and handles. `MinimumGestureSize` (default 5 px) is the minimum bounding-box size for a drag gesture to be treated as a shape rather than a click.

## JSON serialization

`AnnotationSerializer` wraps `System.Text.Json` with all the converters needed for round-tripping:

```csharp
// Serialize
string json = AnnotationSerializer.Serialize(annotationManager.Annotations);
File.WriteAllText("annotations.json", json);

// Deserialize
string json = File.ReadAllText("annotations.json");
var loaded = AnnotationSerializer.DeserializeList(json);

annotationManager.ClearAnnotations();
foreach (var ann in loaded)
    annotationManager.AddAnnotation(ann);
```

Single-annotation helpers also exist:

```csharp
string json    = AnnotationSerializer.Serialize(annotation);
Annotation? ann = AnnotationSerializer.Deserialize(json);
```

All built-in geometry types round-trip correctly. Polymorphic type information is written as a `$type` discriminator in each geometry object.

## Extending with custom shapes

### Custom geometry

Subclass `AnnotationGeometry` and implement `Draw`, `HitTest`, `ApplyImageDelta`, `GetBoundingBox`, and `Clone`. Call `CopyDrawingTo(clone)` inside `Clone()` to preserve the drawing spec.

Register the subtype for JSON serialization by applying `[JsonDerivedType]` to a class that also derives from `Annotation` (or `AnnotationGeometry`), or by registering the type with a custom `JsonSerializerOptions`.

```csharp
public sealed class ArrowAnnotationGeometry : AnnotationGeometry
{
    public Point Tail { get; set; }
    public Point Head { get; set; }

    public override RectangleF GetBoundingBox() => /* ... */;
    public override void Draw(BitmapDisplayPanel panel, Graphics g, bool isSelected) => /* ... */;
    public override AnnotationHitInfo HitTest(BitmapDisplayPanel panel, Point pt, int border) => /* ... */;
    public override void ApplyImageDelta(AnnotationHitInfo hit, Size delta) => /* ... */;
    public override AnnotationGeometry Clone()
    {
        var clone = new ArrowAnnotationGeometry { Tail = Tail, Head = Head };
        CopyDrawingTo(clone);
        return clone;
    }
}
```

### Custom shape recognition

Implement `IAnnotationShapeDescriptor` and register it with the manager:

```csharp
public sealed class ArrowDescriptor : IAnnotationShapeDescriptor
{
    public string Name        => "Arrow";
    public string Description => "Straight arrow between two points";

    public float FitScore(FreehandPath path)
    {
        // Return 0..1; higher means a better match.
        // Use path.BoundingBox, path.ApproximatePerimeter, path.Points, etc.
        return /* ... */;
    }

    public AnnotationGeometry CreateGeometry(FreehandPath path)
    {
        // Fit an arrow to the freehand trace.
        return new ArrowAnnotationGeometry { Tail = /* ... */, Head = /* ... */ };
    }
}

annotationManager.RegisterShapeDescriptor(new ArrowDescriptor());
```

The recogniser ranks all descriptors by `FitScore` and presents the top candidates in the shape menu.

### Custom annotation metadata

Subclass `Annotation` to carry domain-specific fields. Apply `[JsonDerivedType]` to enable polymorphic serialization, and override `Clone()` to copy the extra fields:

```csharp
[JsonDerivedType(typeof(LabelledAnnotation), "labelled")]
public class LabelledAnnotation : Annotation
{
    public float Confidence { get; set; }

    public LabelledAnnotation(AnnotationGeometry geometry) : base(geometry) { }

    public override Annotation Clone()
    {
        var clone = (LabelledAnnotation)base.Clone();
        clone.Confidence = Confidence;
        return clone;
    }
}
```
