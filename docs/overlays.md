# Overlays

Overlays let you draw static shapes over (or under) an image without subclassing `BitmapDisplayPanel`. Each shape is drawn from within a `PaintOver` or `PaintUnder` event handler. Shapes have no internal `Drawing` state — you pass a `DrawingSpec` to each `Draw()` call, which means multiple shapes can share the same spec.

## Quick example

```csharp
var spec = new DrawingSpec();
spec.Lines.Color = Color.Lime;
spec.Lines.Width = 2;
spec.Fill.Color  = Color.FromArgb(60, Color.Lime);

var box = new RectangleShape { Rect = new RectangleF(100, 50, 200, 150) };

bitmapDisplayPanel.PaintOver += (sender, e) =>
    box.Draw((BitmapDisplayPanel)sender!, e.Graphics, spec);
```

## Available shapes

| Type | Key properties |
|---|---|
| `RectangleShape` | `Rect` — axis-aligned rectangle in image coordinates |
| `RotatedRectangleShape` | `Centre`, `Width`, `Height`, `Angle` — clockwise rotation in degrees |
| `CircleShape` | `Centre`, `Radius` |
| `EllipseShape` | `Centre`, `MajorAxis`, `MinorAxis`, `MajorAxisAngleDegrees` |
| `DonutShape` | `Centre`, `SemiMajorAxis`, `SemiMinorAxis`, `MajorAxisAngleDegrees`, `Thickness`, `StartAngle`, `SweepAngle` |
| `LineShape` | `Start`, `End` |
| `PolygonShape` | `SetPoints(PointF[])` — closed polygon |
| `TextShape` | `Location`, `Text` |
| `CrosshairShape` | `Centre`, `Length`, `CentreGap` — all in image coordinates |

All shapes expose `void Draw(BitmapDisplayPanel panel, Graphics graphics, DrawingSpec drawing)`.

All shapes have a `PixelAlign` property (`DisplayPixelAlign.Centre` by default) that controls sub-pixel alignment when `MappingMode` is `ImageToDisplay`.

## DonutShape

`DonutShape` draws an elliptical ring or arc slice. It is centred on an ellipse and rendered between an inner and outer ellipse each offset from the reference by half of `Thickness`. A `SweepAngle` of 360° produces a full ring; any smaller value produces an arc slice.

```csharp
// Full elliptical ring
var ring = new DonutShape
{
    Centre              = new PointF(300, 200),
    SemiMajorAxis       = 80,
    SemiMinorAxis       = 50,
    MajorAxisAngleDegrees = 20,
    Thickness           = 20,
    SweepAngle          = 360,
};

// 270° arc slice
var slice = new DonutShape
{
    Centre        = new PointF(500, 400),
    SemiMajorAxis = 60,
    SemiMinorAxis = 60,
    StartAngle    = 0,
    SweepAngle    = 270,
    Thickness     = 25,
};

var spec = new DrawingSpec();
spec.Lines.Color = Color.Green;
spec.Fill.Color  = Color.FromArgb(64, Color.LimeGreen);

bitmapDisplayPanel.PaintOver += (sender, e) =>
{
    var panel = (BitmapDisplayPanel)sender!;
    ring.Draw(panel, e.Graphics, spec);
    slice.Draw(panel, e.Graphics, spec);
};
```

`StartAngle` and `SweepAngle` follow the GDI+ convention: 0° is the positive x-axis (3 o'clock), values increase clockwise. Negative `SweepAngle` sweeps counter-clockwise.

## DrawingSpec

`DrawingSpec` is passed to `Draw()` and bundles all visual properties:

| Property | Type | Purpose |
|---|---|---|
| `Lines` | `PenSpec` | Outline colour, width, dash style |
| `Fill` | `BrushSpec` | Fill colour (alpha supported) |
| `Font` | `FontSpec` | Font name, size, style (used by `TextShape`) |
| `Visible` | `bool` | Show/hide without removing the shape |
| `MappingMode` | `MappingMode` | How coordinates are interpreted (see below) |

### MappingMode

| Value | Behaviour |
|---|---|
| `ImageToDisplay` | Coordinates are in image pixels — the shape tracks the image as you zoom and pan |
| `DirectToDisplay` | Coordinates are in display pixels — the shape stays fixed on screen |

### PenSpec

```csharp
spec.Lines.Color     = Color.Red;
spec.Lines.Width     = 2f;
spec.Lines.DashStyle = DashStyle.Dash;
```

### BrushSpec

```csharp
spec.Fill.Color = Color.FromArgb(80, Color.Yellow);  // semi-transparent
```

### FontSpec

```csharp
spec.Font.FontName = "Segoe UI";
spec.Font.FontSize = 10f;
spec.Font.Bold     = true;
```

## DrawingToolsPool

`DrawingToolsPool` caches `Pen`, `Brush`, and `Font` objects by spec equality so paint-loop code doesn't allocate GDI+ objects on every frame. Shapes use it internally; you can also call it directly:

```csharp
Pen   pen   = DrawingToolsPool.GetPen(spec.Lines);
Brush brush = DrawingToolsPool.GetBrush(spec.Fill);
Font  font  = DrawingToolsPool.GetFont(spec.Font);
```

The pool is process-wide and thread-safe for reads.

## TextPanelStd

`TextPanelStd` draws a rounded, semi-transparent information panel in the top-left corner of the control:

```csharp
private readonly TextPanelStd _textPanel = new();

bitmapDisplayPanel.PaintOver += (sender, e) =>
{
    _textPanel.Clear();
    _textPanel.AddMessage(TextPanelStdMsgTypes.Title,   "Inspection result");
    _textPanel.AddMessage(TextPanelStdMsgTypes.Info,    $"Position: {x}, {y}");
    _textPanel.AddMessage(TextPanelStdMsgTypes.Warning, "Near limit");
    _textPanel.AddMessage(TextPanelStdMsgTypes.Error,   "Out of tolerance");
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

Customise any type via `DrawingSpecs`:

```csharp
_textPanel.DrawingSpecs.Title.Font.FontSize = 14;
_textPanel.DrawingSpecs.Panel.Fill.Color = Color.FromArgb(160, Color.DarkSlateBlue);
```

For custom message types, use `TextPanel<TMessageType>` with your own enum and a delegate that maps each value to a `DrawingSpec`.
