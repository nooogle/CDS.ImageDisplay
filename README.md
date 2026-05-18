# CDS.ImageDisplay

A Windows Forms library for displaying, zooming, panning, and annotating bitmaps. Designed for use in imaging and machine-vision applications where you need a responsive, thread-safe image viewer with overlay graphics and interactive region-of-interest (ROI) selection.

## Requirements

- .NET 10, Windows
- Windows Forms

## Installation

```
dotnet add package CDS.ImageDisplay.WinForms
```

## Quick start

```csharp
// Drop BitmapDisplayPanel onto a form (designer or code), then:
bitmapDisplayPanel.DisplayMode = BitmapDisplayMode.FitToWindowCentred;

using var bitmap = new Bitmap("photo.jpg");
bitmapDisplayPanel.SetImage(bitmap);   // safe to call from any thread
```

## Features

### Display modes

| Mode | Behaviour |
|---|---|
| `FitToWindowCentred` | Scales to fill the control, maintaining aspect ratio |
| `ActualSizeCentred` | 1:1 zoom, centred |
| `Free` | User-controlled pan and zoom |

Set via the `DisplayMode` property (also available in the WinForms designer under the **CDS** category).

### Zoom and pan (`Free` mode)

- **Scroll wheel** — zoom in/out, anchored to the cursor position
- **Left drag** — pan
- **Arrow keys** — pan (the control must have keyboard focus)

Programmatic helpers:

```csharp
panel.ZoomIn();
panel.ZoomOut();
panel.ResetZoom();          // back to 1:1
panel.CentreImage();        // re-centre without changing zoom
panel.FitToWindowCentred(); // fit and centre
```

Synchronise two panels (e.g. for side-by-side comparison):

```csharp
panel2.SyncPaintRectFromOther(panel1);
```

### Thread-safe image updates

`SetImage` accepts either a `Bitmap` or an `IImageSource`. When called from a background thread it queues the image for the UI thread and returns immediately; rapid producers coalesce to one pending update so the message loop cannot accumulate a backlog.

```csharp
// In a camera capture loop on a worker thread:
bitmapDisplayPanel.SetImage(frame);
```

### Paint hooks

Subscribe to `OnPaintOver` or `OnPaintUnder` for flicker-free custom drawing. Both events receive a `Graphics` object tied to the double-buffered paint cycle.

```csharp
bitmapDisplayPanel.OnPaintOver += (sender, e) =>
{
    var panel = (BitmapDisplayPanel)sender!;
    // draw over the image here
};
```

### Overlays

Shapes are drawn through the paint-hook events. Each shape carries a `DrawingSpec` (pen, brush, font, visibility) and a `MappingMode`:

| `MappingMode` | Coordinates |
|---|---|
| `ImageToDisplay` | Track the image — scale and pan with zoom |
| `DirectToDisplay` | Fixed screen coordinates |

```csharp
var box = new RectangleShape { ImageRect = new RectangleF(100, 50, 200, 150) };
box.Drawing.Lines.Color = Color.Lime;
box.Drawing.Lines.Width = 2;

bitmapDisplayPanel.OnPaintOver += (sender, e) =>
    box.Draw((BitmapDisplayPanel)sender!, e.Graphics);
```

Available shapes: `RectangleShape`, `CircleShape`, `EllipseShape`, `LineShape`, `PolygonShape`, `TextShape`, `CrosshairShape`.

`DrawingToolsPool` caches `Pen`, `Brush`, and `Font` objects by spec equality, avoiding GDI+ allocations in paint loops.

### Regions of interest

`SingleROIManager` is a WinForms component that adds an interactive, resizable rectangle to a `BitmapDisplayPanel`. Drop it on a form alongside the panel, set `BitmapDisplayPanel`, and subscribe to events:

```csharp
singleROIManager.BitmapDisplayPanel = bitmapDisplayPanel;

singleROIManager.OnCommittedROIChanged += (s, e) =>
{
    // e.CommittedROI is in image coordinates, independent of zoom/pan
    Console.WriteLine(e.CommittedROI);
};
```

- `OnDraggingROIChanged` — fires continuously while the user drags
- `OnCommittedROIChanged` — fires on mouse-up
- `DragBorder` — pixel tolerance around edges and corners for hit-testing

`MultipleROIManager` manages a list of `ISingleROIDescriptor` objects with one active at a time. Implement `ISingleROIDescriptor` to add custom ROI types.

### Coordinate mapping

```csharp
// Image pixel under the mouse:
PointF imagePoint = panel.MapDisplayToImage(e.Location);

// Screen rectangle for an image region:
Rectangle screenRect = panel.MapImageToDisplay(
    new RectangleF(x, y, w, h),
    DisplayPixelAlign.TopLeft);
```

Three coordinate spaces exist: *image* (source bitmap pixels), *display* (control client area), and *paint-rect* (the sub-rectangle where the image is drawn). `PaintRect` and `SizeOfHalfDisplayPixel` give you the geometry for custom drawing.

### Custom image sources

Implement `IImageSource` to feed a native pixel buffer without creating an intermediate `Bitmap` — useful for OpenCV `Mat` or other image types that expose a pointer to their data:

```csharp
public class MatImageSource : IImageSource
{
    private readonly Mat _mat;
    public MatImageSource(Mat mat) => _mat = mat;
    public bool IsImageAvailable => true;
    public int Width  => _mat.Width;
    public int Height => _mat.Height;
    public int Stride => (int)_mat.Step();
    public Size Size  => new(_mat.Width, _mat.Height);
    public IntPtr Scan0 => _mat.Data;
    public PixelFormat PixelFormat => PixelFormat.Format24bppRgb;
}

bitmapDisplayPanel.SetImage(new MatImageSource(mat));
```

## Solution layout

| Project | Role |
|---|---|
| `CDS.ImageDisplay.WinForms` | Library — the NuGet deliverable |
| `UnitTests` | MSTest + AwesomeAssertions unit tests |
| `BenchmarkTests` | BenchmarkDotNet benchmarks (Release config) |
| `CDS.ImageDisplay.Demo` | WinForms demo app (uses OpenCvSharp4) |

## Building

```
dotnet build
dotnet test
dotnet pack CDS.ImageDisplay.WinForms
```

Versioning is handled by [MinVer](https://github.com/adamralph/minver) using a `V` tag prefix (e.g. `V2.2.0`).

## License

[MIT](LICENSE)
