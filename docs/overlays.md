# Overlays

Overlays let you draw static shapes over (or under) an image without subclassing `BitmapDisplayPanel`. Each shape is drawn from within a `PaintOver` or `PaintUnder` event handler.

## Quick example

```csharp
var box = new RectangleShape { ImageRect = new RectangleF(100, 50, 200, 150) };
box.Drawing.Lines.Color = Color.Lime;
box.Drawing.Lines.Width = 2;

bitmapDisplayPanel.PaintOver += (sender, e) =>
    box.Draw((BitmapDisplayPanel)sender!, e.Graphics);
```

## Available shapes

| Type | Key properties |
|---|---|
| `RectangleShape` | `ImageRect` — axis-aligned rectangle in image coordinates |
| `CircleShape` | `ImageCentre`, `ImageRadius` |
| `EllipseShape` | `ImageRect` — bounding rectangle of the ellipse |
| `LineShape` | `ImageStart`, `ImageEnd` |
| `PolygonShape` | `ImagePoints` — list of vertices |
| `TextShape` | `ImageLocation`, `Text` |
| `CrosshairShape` | `ImageCentre` — arms sized in display pixels so they stay constant across zoom |

All shapes call `Draw(BitmapDisplayPanel panel, Graphics graphics)` and get their coordinates from whatever `MappingMode` is configured.

## DrawingSpec

Every shape has a `Drawing` property of type `DrawingSpec`, which bundles:

| Property | Type | Purpose |
|---|---|---|
| `Lines` | `PenSpec` | Outline colour, width, dash style |
| `Fill` | `BrushSpec` | Fill colour (alpha supported) |
| `Font` | `FontSpec` | Font name, size, style (for `TextShape`) |
| `Visible` | `bool` | Show/hide without removing the shape |
| `MappingMode` | `MappingMode` | How coordinates are interpreted (see below) |

### MappingMode

| Value | Behaviour |
|---|---|
| `ImageToDisplay` | Coordinates are in image pixels — the shape tracks the image as you zoom and pan |
| `DirectToDisplay` | Coordinates are in display pixels — the shape stays fixed on screen |

### PenSpec

```csharp
shape.Drawing.Lines.Color     = Color.Red;
shape.Drawing.Lines.Width     = 2f;
shape.Drawing.Lines.DashStyle = DashStyle.Dash;
```

### BrushSpec

```csharp
shape.Drawing.Fill.Color = Color.FromArgb(80, Color.Yellow);  // semi-transparent
```

### FontSpec

```csharp
textShape.Drawing.Font.FontName = "Segoe UI";
textShape.Drawing.Font.FontSize = 10f;
textShape.Drawing.Font.Bold     = true;
```

## DrawingToolsPool

`DrawingToolsPool` caches `Pen`, `Brush`, and `Font` objects by spec equality, so paint-loop code doesn't allocate GDI+ objects on every frame. Shapes use it internally; you can also use it directly:

```csharp
Pen pen     = DrawingToolsPool.GetPen(mySpec.Lines);
Brush brush = DrawingToolsPool.GetBrush(mySpec.Fill);
Font font   = DrawingToolsPool.GetFont(mySpec.Font);
```

The pool is process-wide and thread-safe for reads.

## TextPanelStd

`TextPanelStd` draws a rounded, semi-transparent information panel in the top-left corner of the control. Call it from a paint event:

```csharp
private readonly TextPanelStd _textPanel = new();

bitmapDisplayPanel.PaintOver += (sender, e) =>
{
    _textPanel.Clear();
    _textPanel.AddMessage(TextPanelStdMsgTypes.Title, "Inspection result");
    _textPanel.AddMessage(TextPanelStdMsgTypes.Info, $"Position: {x}, {y}");
    _textPanel.AddMessage(TextPanelStdMsgTypes.Warning, "Near limit");
    _textPanel.AddMessage(TextPanelStdMsgTypes.Error, "Out of tolerance");
    _textPanel.Draw((BitmapDisplayPanel)sender!, e.Graphics);
};
```

Built-in message types and their default rendering:

| Type | Appearance |
|---|---|
| `Title` | Larger font, white |
| `Info` | Normal font, light grey |
| `Success` | Normal font, green |
| `Warning` | Normal font, yellow |
| `Error` | Normal font, red |

Customise any message type via `DrawingSpecs`:

```csharp
_textPanel.DrawingSpecs.Title.Font.FontSize = 14;
_textPanel.DrawingSpecs.Panel.Fill.Color = Color.FromArgb(160, Color.DarkSlateBlue);
```

For custom message types, use `TextPanel<TMessageType>` directly with your own enum and a delegate that maps each value to a `DrawingSpec`.
