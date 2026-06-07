# Display panel

`BitmapDisplayPanel` is the central control. Drop it on a form (designer or code), set a display mode, and call `SetImage`.

```csharp
bitmapDisplayPanel.DisplayMode = BitmapDisplayMode.FitToWindowCentred;

using var bitmap = new Bitmap("photo.jpg");
bitmapDisplayPanel.SetImage(bitmap);   // safe to call from any thread
```

## Display modes

| Mode | Behaviour |
|---|---|
| `FitToWindowCentred` | Scales to fill the control, maintaining aspect ratio |
| `ActualSizeCentred` | 1:1 zoom, centred |
| `Free` | User-controlled pan and zoom |

Set via the `DisplayMode` property (also available in the WinForms designer under the **CDS** category).

## Zoom and pan

Zoom and pan are available in `Free` mode.

**Mouse and keyboard interactions:**

| Action | Effect |
|---|---|
| Scroll wheel | Zoom in/out, anchored to the cursor position |
| Left drag | Pan |
| Arrow keys | Pan (control must have keyboard focus) |
| Spacebar + left drag | Pan while another interaction (e.g. an annotation) is active |

**Programmatic control:**

```csharp
panel.ZoomIn();
panel.ZoomOut();
panel.ResetZoom();           // back to 1:1
panel.CentreImage();         // re-centre without changing zoom
panel.FitToWindowCentred();  // fit and centre
```

**Synchronise two panels** (e.g. for side-by-side comparison):

```csharp
panel2.SyncPaintRectFromOther(panel1);
```

`ZoomManager` snaps exactly to 1.0× within a small tolerance so a single step never produces 0.999× or 1.001×.

## Thread-safe image updates

`SetImage` accepts either a `Bitmap` or an `IImageSource`. When called from a background thread it queues the image for the UI thread and returns immediately. Rapid producers coalesce to one pending update so the message loop cannot accumulate a backlog.

```csharp
// In a camera capture loop on a worker thread:
bitmapDisplayPanel.SetImage(frame);
```

## Coordinate mapping

Three coordinate spaces exist:

| Space | Description |
|---|---|
| *image* | Pixels in the source bitmap |
| *display* | Pixels in the control's client area |
| *paint-rect* | The sub-rectangle of the client area where the image is actually rendered |

Mapping methods:

```csharp
// Image pixel under the mouse:
PointF imagePoint = panel.MapDisplayToImage(e.Location);

// Screen rectangle for an image region:
Rectangle screenRect = panel.MapImageToDisplay(
    new RectangleF(x, y, w, h),
    DisplayPixelAlign.TopLeft);
```

`PaintRect` gives you the current paint-rect, and `SizeOfHalfDisplayPixel` is useful when you need pixel-perfect alignment in custom drawing.

## Custom image sources

Implement `IImageSource` to feed a native pixel buffer without creating an intermediate `Bitmap`. This is useful for OpenCV `Mat` or other image types that expose a pointer to their data:

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

## Paint hooks

Subscribe to `PaintOver` or `PaintUnder` for flicker-free custom drawing. Both events receive a `Graphics` object tied to the double-buffered paint cycle.

```csharp
bitmapDisplayPanel.PaintOver += (sender, e) =>
{
    e.Graphics.DrawLine(Pens.Red, 0, 0, 100, 100);
};
```

`PaintUnder` fires before the image is composited; `PaintOver` fires after. Both pass a `PaintOverEventArgs` with `Sender` (the panel), `Graphics`, and `PaintRect`.

`PaintRectChanged` fires whenever the region occupied by the image changes (e.g. on zoom, pan, or resize), which is useful for synchronising external overlays.
