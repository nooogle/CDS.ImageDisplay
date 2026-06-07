# Regions of interest

## SingleROIManager

`SingleROIManager` is a WinForms component that manages one interactive, resizable rectangle on a `BitmapDisplayPanel`. Drop it on a form alongside the panel and wire them up:

```csharp
singleROIManager.BitmapDisplayPanel = bitmapDisplayPanel;
```

The ROI tracks the image as you zoom and pan — its coordinates are always in image pixels.

### Events

```csharp
singleROIManager.CommittedROIChanged += (s, e) =>
{
    // Fires on mouse-up; e.ROI is in image coordinates
    Console.WriteLine(e.ROI);
};

singleROIManager.DraggingROIChanged += (s, e) =>
{
    // Fires continuously while dragging
};
```

### Programmatic access

```csharp
// Read the current ROI
RectangleF roi = singleROIManager.CommittedROI;

// Set programmatically
singleROIManager.CommittedROI = new RectangleF(100, 100, 200, 150);
```

### Configuration

| Property | Default | Description |
|---|---|---|
| `DragBorder` | 8 | Hit-test tolerance in display pixels around edges and corners |
| `MinSize` | — | Minimum ROI size in image pixels |
| `MaxSize` | — | Maximum ROI size in image pixels |

## MultipleROIManager

`MultipleROIManager` manages a list of `ISingleROIDescriptor` objects with one active at a time. The active descriptor receives mouse events; the rest are rendered as inactive overlays.

```csharp
// Implement ISingleROIDescriptor for each ROI type, then:
multipleROIManager.BitmapDisplayPanel = bitmapDisplayPanel;
multipleROIManager.AddDescriptor(new MyROIDescriptor());
```

Implement `ISingleROIDescriptor` to create custom ROI types with their own visual style, constraints, and state.

`ROIWithGrapplesShape` is the built-in implementation; it renders a rectangle with eight grapple handles and enforces optional min/max size constraints.

## Line selection

`SingleLineSelectionManager` is a WinForms component that adds an interactive, repositionable line to a `BitmapDisplayPanel`.

```csharp
singleLineSelectionManager.BitmapDisplayPanel = bitmapDisplayPanel;

singleLineSelectionManager.CommittedLineChanged += (s, e) =>
{
    // e.Line is a (Point Start, Point End) tuple in image coordinates
    Console.WriteLine($"{e.Line.Start} → {e.Line.End}");
};
```

### Interactions

| Action | Effect |
|---|---|
| Left-click and drag on empty area | Draw a new line |
| Drag the line body | Move the whole line |
| Drag an endpoint handle | Reposition that endpoint |
| **Spacebar** + drag | Pan while a line is active |

### Events

| Event | Fires when |
|---|---|
| `CommittedLineChanged` | Mouse-up; `e.Line` carries final endpoints in image coordinates |
| `DraggingLineChanged` | Continuously during drag |

### Programmatic access

```csharp
// Set the line
singleLineSelectionManager.CommittedLine = (new Point(10, 20), new Point(200, 150));

// Clear it
singleLineSelectionManager.CommittedLine = null;
```

### Configuration

| Property | Description |
|---|---|
| `DragBorder` | Hit-test tolerance in display pixels around endpoints and the line body (default 10) |
| `CanCreateNew` | Allow drawing a new line |
| `CanEditCommitted` | Allow moving or resizing the existing line |
| `CommittedLineDrawingSpec` | Visual style for the committed line |
| `LiveDraggingLineDrawingSpec` | Visual style while dragging |
