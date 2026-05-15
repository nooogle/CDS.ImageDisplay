using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using CDS.ImageDisplay.BitmapDisplay;
using Humanizer;

namespace CDS.ImageDisplay.Demo.DemoForms;

internal sealed partial class FormPaintOverAndUnder : Form
{
    private Bitmap? loadedBitmap;
    private readonly Font fixedWidthFont;
    private readonly Brush msgPanelBrush;
    private readonly Overlays.CrosshairShape crossHair = new();

    public FormPaintOverAndUnder()
    {
        InitializeComponent();

        fixedWidthFont = new Font(family: FontFamily.GenericMonospace, emSize: 10);
        msgPanelBrush = new SolidBrush(Color.FromArgb(128, Color.Navy));
    }

    private void FormBitmapDisplay_Load(object sender, EventArgs e)
    {
        menuImageBuiltIn.Items.Add(new ImageNameAndResource("Budapest", Properties.Resources.Budapest_8U1C));
        menuImageBuiltIn.Items.Add(new ImageNameAndResource("16*16", Properties.Resources._16x16_24_bit));
        menuImageBuiltIn.Items.Add(new ImageNameAndResource("1*1", Properties.Resources._1x1_24_bit));
        menuImageBuiltIn.Items.Add(new ImageNameAndResource("Alpha-channel", Properties.Resources.AlphaEdge));
        menuImageBuiltIn.SelectedIndex = 0;

        UpdateCommandEnablement();
        UpdateDisplayModeCheckboxes();
    }


    private void bitmapDisplay_PaintOver(object sender, PaintOverEventArgs e)
    {
        var bitmapDisplay = (BitmapDisplayPanel)sender;
        var graphics = e.Graphics;
        var info = new StringBuilder();
        info.Append(CultureInfo.CurrentCulture, $"Display mode      {bitmapDisplay.DisplayMode.Humanize()}\n");
        info.Append(CultureInfo.CurrentCulture, $"Display size      {bitmapDisplay.ClientSize}\n");
        if (!bitmapDisplay.AnythingToDisplay)
        {
            info.Append("Image not loaded\n");
        }
        else
        {
            RectangleF r = bitmapDisplay.PaintRect;
            info.Append(CultureInfo.CurrentCulture, $"Bitmap size       {bitmapDisplay.DisplayImage?.Size}\n");
            info.Append(CultureInfo.CurrentCulture, $"Paint zoom        {bitmapDisplay.Zoom:0.000}\n");
            info.Append(CultureInfo.CurrentCulture, $"Paint rect        {r.X:0.0}, {r.Y:0.0}, {r.Width:0.0}, {r.Height:0:0}\n");
            info.Append(CultureInfo.CurrentCulture, $"Format            {bitmapDisplay.DisplayImage?.PixelFormat.Humanize()}\n");
        }
        info.Append(CultureInfo.CurrentCulture, $"Paint foreground  {bitmapDisplay.TimingMetrics.ForegroundPaint.Humanize()}\n");
        info.Append(CultureInfo.CurrentCulture, $"Paint background  {bitmapDisplay.TimingMetrics.BackgroundPaint.Humanize()}\n");

        var textTopleft = new PointF(12, 12);
        SizeF textBlockSize = graphics.MeasureString(info.ToString(), fixedWidthFont);

        var bkRect = new RectangleF(
            textTopleft,
            textBlockSize);

        graphics.FillRectangle(msgPanelBrush, bkRect);
        graphics.DrawRectangle(Pens.Navy, Rectangle.Round(bkRect));

        graphics.DrawString(
            info.ToString(),
            fixedWidthFont,
            Brushes.Yellow,
            textTopleft);

        Rectangle topLeftBox = bitmapDisplay.MapImageToDisplay(new RectangleF(0, 0, 10, 5), DisplayPixelAlign.TopLeft);
        graphics.DrawRectangle(Pens.Red, topLeftBox.X, topLeftBox.Y, topLeftBox.Width, topLeftBox.Height);

        graphics.DrawString(
            "A 10*5 box",
            fixedWidthFont,
            Brushes.Navy,
            new PointF(topLeftBox.X, topLeftBox.Y));

        var crossHairDrawingSpec = new Overlays.DrawingSpec();
        crossHairDrawingSpec.Lines.Color = Color.Yellow;

        crossHair.Centre = bitmapDisplay.TargetImageCentre;
        crossHair.Draw(bitmapDisplay, graphics, crossHairDrawingSpec);


        // Line test
        graphics.DrawLine(
            Pens.Black,
            bitmapDisplay.MapImageToDisplay(new Point(50, 50), DisplayPixelAlign.Centre),
            bitmapDisplay.MapImageToDisplay(new Point(100, 60), DisplayPixelAlign.Centre));
    }

    private void menuDisplayModeFree_Click(object sender, EventArgs e)
    {
        bitmapDisplay.DisplayMode = BitmapDisplayMode.Free;
        UpdateCommandEnablement();
        bitmapDisplay.Invalidate();
    }

    private void UpdateCommandEnablement()
    {
        menuDisplayCentre.Enabled = bitmapDisplay.DisplayMode == BitmapDisplayMode.Free;
        menuDisplayZoomIn.Enabled = bitmapDisplay.DisplayMode == BitmapDisplayMode.Free;
        menuDisplayZoomOut.Enabled = bitmapDisplay.DisplayMode == BitmapDisplayMode.Free;
        menuDisplayZoomReset.Enabled = bitmapDisplay.DisplayMode == BitmapDisplayMode.Free;
    }

    private void menuDisplayModeFitToWindow_Click(object sender, EventArgs e)
    {
        bitmapDisplay.DisplayMode = BitmapDisplayMode.FitToWindowCentred;
        UpdateCommandEnablement();
        bitmapDisplay.Invalidate();
    }

    private void menuDisplayModeActualSize_Click(object sender, EventArgs e)
    {
        bitmapDisplay.DisplayMode = BitmapDisplayMode.ActualSizeCentred;
        UpdateCommandEnablement();
        bitmapDisplay.Invalidate();
    }

    private void menuDisplayCentre_Click(object sender, EventArgs e) => bitmapDisplay.CentreImage();


    private void MenuImageBuiltIn_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        var imageNameAndResource = menuImageBuiltIn.SelectedItem as ImageNameAndResource;

        bitmapDisplay.SetImage(imageNameAndResource?.Bitmap);
    }


    private void bitmapDisplay_PaintUnder(object sender, PaintUnderEventArgs e)
    {
        var bitmapDisplay = (BitmapDisplayPanel)sender;
        var graphics = e.Graphics;
        if (bitmapDisplay.PaintRect.IsEmpty)
        { return; }

        graphics.DrawLine(
            Pens.Navy,
            bitmapDisplay.PaintRect.Location,
            new PointF(bitmapDisplay.PaintRect.Right - 1, bitmapDisplay.PaintRect.Bottom - 1));

        graphics.DrawLine(
            Pens.Navy,
            new PointF(bitmapDisplay.PaintRect.Right - 1, bitmapDisplay.PaintRect.Top),
            new PointF(bitmapDisplay.PaintRect.Left, bitmapDisplay.PaintRect.Bottom - 1));
    }


    private void MenuImageExit_Click(object sender, System.EventArgs e) => Close();

    private void MenuImageOpen_Click(object sender, System.EventArgs e)
    {
        if (openImageDialog.ShowDialog(this) == DialogResult.OK)
        {
            OpenFromFile(openImageDialog.FileName);
        }
    }


    private void OpenFromFile(string fileName)
    {
        try
        {
            DropLoadedBitmap();
            loadedBitmap = (Bitmap)Image.FromFile(fileName);
            bitmapDisplay.SetImage(loadedBitmap);
        }
#pragma warning disable CA1031 // Broad catch is intentional: any image-load failure should be shown to the user
        catch (Exception exception)
        {
            MessageBox.Show(
                owner: this,
                text: $"Failed to load an image or send to the display: {exception.Message}",
                caption: Application.ProductName,
                buttons: MessageBoxButtons.OK,
                icon: MessageBoxIcon.Error);
        }
#pragma warning restore CA1031
    }


    private void DropLoadedBitmap()
    {
        loadedBitmap?.Dispose();
        loadedBitmap = null;
    }


    private void bitmapDisplay_DisplayModeChanged(object sender, EventArgs e)
    {
        UpdateCommandEnablement();
        UpdateDisplayModeCheckboxes();
    }


    public void UpdateDisplayModeCheckboxes()
    {
        menuDisplayModeFitToWindow.Checked = bitmapDisplay.DisplayMode == BitmapDisplayMode.FitToWindowCentred;
        menuDisplayModeActualSize.Checked = bitmapDisplay.DisplayMode == BitmapDisplayMode.ActualSizeCentred;
        menuDisplayModeFree.Checked = bitmapDisplay.DisplayMode == BitmapDisplayMode.Free;
    }

    private void menuDisplayZoomOut_Click(object sender, System.EventArgs e) => bitmapDisplay.ZoomOut();

    private void menuDisplayZoomIn_Click(object sender, System.EventArgs e) => bitmapDisplay.ZoomIn();

    private void menuDisplayZoomReset_Click(object sender, System.EventArgs e) => bitmapDisplay.ResetZoom();


    private void MenuDisplayActualSize_Click(object sender, System.EventArgs e) => bitmapDisplay.CentreImageActualSize();

    private void MenuDisplayFitToWindow_Click(object sender, System.EventArgs e) => bitmapDisplay.FitToWindowCentred();

}
