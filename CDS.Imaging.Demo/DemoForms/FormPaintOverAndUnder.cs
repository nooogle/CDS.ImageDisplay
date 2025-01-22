using CDS.Imaging.BitmapDisplay;
using Humanizer;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CDS.Imaging.Demo.DemoForms
{
    public partial class FormPaintOverAndUnder : Form
    {
        Bitmap? loadedBitmap;
        Font fixedWidthFont;
        Brush msgPanelBrush;
        Draw.CrosshairShape crossHair = new Draw.CrosshairShape();


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


        private void bitmapDisplay_PaintOver(BitmapDisplayPanel sender, Graphics graphics)
        {
            var info = new StringBuilder();
            info.Append($"Display mode      {sender.DisplayMode.Humanize()}\n");
            info.Append($"Display size      {sender.ClientSize}\n");
            if (!sender.AnythingToDisplay)
            {
                info.Append($"Image not loaded\n");
            }
            else
            {
                var r = sender.PaintRect;
                info.Append($"Bitmap size       {sender.GetDisplayImage()?.Size}\n");
                info.Append($"Paint zoom        {sender.Zoom:0.000}\n");
                info.Append($"Paint rect        {r.X:0.0}, {r.Y:0.0}, {r.Width:0.0}, {r.Height:0:0}\n");
                info.Append($"Format            {sender.GetDisplayImage()?.PixelFormat.Humanize()}\n");
            }
            info.Append($"Set image         {sender.TimingMetrics.SetImage.Humanize()}\n");
            info.Append($"Paint foreground  {sender.TimingMetrics.ForegroundPaint.Humanize()}\n");
            info.Append($"Paint background  {sender.TimingMetrics.BackgroundPaint.Humanize()}\n");

            var textTopleft = new PointF(12, 12);
            var textBlockSize = graphics.MeasureString(info.ToString(), fixedWidthFont);

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

            var topLeftBox = sender.MapImageToDisplay(new RectangleF(0, 0, 10, 5), DisplayPixelAlign.TopLeft);
            graphics.DrawRectangle(Pens.Red, topLeftBox.X, topLeftBox.Y, topLeftBox.Width, topLeftBox.Height);

            graphics.DrawString(
                "A 10*5 box",
                fixedWidthFont,
                Brushes.Navy,
                new PointF(topLeftBox.X, topLeftBox.Y));

            crossHair.Centre = bitmapDisplay.TargetImageCentre;
            crossHair.Draw(bitmapDisplay, graphics);


            // Line test
            graphics.DrawLine(
                Pens.Black,
                sender.MapImageToDisplay(new Point(50, 50), DisplayPixelAlign.Centre),
                sender.MapImageToDisplay(new Point(100, 60), DisplayPixelAlign.Centre));
        }

        private void menuDisplayModeFree_Click(object sender, EventArgs e)
        {
            bitmapDisplay.DisplayMode = BitmapDisplayMode.Free;
            UpdateCommandEnablement();
            bitmapDisplay.Invalidate();
        }

        private void UpdateCommandEnablement()
        {
            menuDisplayCentre.Enabled = (bitmapDisplay.DisplayMode == BitmapDisplayMode.Free);
            menuDisplayZoomIn.Enabled = (bitmapDisplay.DisplayMode == BitmapDisplayMode.Free);
            menuDisplayZoomOut.Enabled = (bitmapDisplay.DisplayMode == BitmapDisplayMode.Free);
            menuDisplayZoomReset.Enabled = (bitmapDisplay.DisplayMode == BitmapDisplayMode.Free);
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

        private void menuDisplayCentre_Click(object sender, EventArgs e)
        {
            bitmapDisplay.CentreImage();
        }


        private void MenuImageBuiltIn_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var imageNameAndResource = menuImageBuiltIn.SelectedItem as ImageNameAndResource;

            bitmapDisplay.SetImage(imageNameAndResource?.Bitmap);
        }


        private void bitmapDisplay_PaintUnder(BitmapDisplayPanel sender, Graphics graphics)
        {
            if(sender.PaintRect.IsEmpty) { return; }

            graphics.DrawLine(
                Pens.Navy, 
                sender.PaintRect.Location, 
                new PointF(sender.PaintRect.Right - 1, sender.PaintRect.Bottom - 1));

            graphics.DrawLine(
                Pens.Navy, 
                new PointF(sender.PaintRect.Right - 1, sender.PaintRect.Top), 
                new PointF(sender.PaintRect.Left, sender.PaintRect.Bottom - 1));
        }


        private void MenuImageExit_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void MenuImageOpen_Click(object sender, System.EventArgs e)
        {
            if(openImageDialog.ShowDialog(this) == DialogResult.OK)
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
            catch(Exception exception)
            {
                MessageBox.Show(
                    owner: this,
                    text: $"Failed to load an image or send to the display: {exception.Message}",
                    caption: Application.ProductName,
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error);
            }
        }


        private void DropLoadedBitmap()
        {
            loadedBitmap?.Dispose();
            loadedBitmap = null;
        }


        private void bitmapDisplay_DisplayModeChanged(BitmapDisplayPanel sender)
        {
            UpdateCommandEnablement();
            UpdateDisplayModeCheckboxes();
        }


        public void UpdateDisplayModeCheckboxes()
        {
            menuDisplayModeFitToWindow.Checked = (bitmapDisplay.DisplayMode == BitmapDisplayMode.FitToWindowCentred);
            menuDisplayModeActualSize.Checked = (bitmapDisplay.DisplayMode == BitmapDisplayMode.ActualSizeCentred);
            menuDisplayModeFree.Checked = (bitmapDisplay.DisplayMode == BitmapDisplayMode.Free);
        }

        private void menuDisplayZoomOut_Click(object sender, System.EventArgs e)
        {
            bitmapDisplay.ZoomOut();
        }

        private void menuDisplayZoomIn_Click(object sender, System.EventArgs e)
        {
            bitmapDisplay.ZoomIn();
        }

        private void menuDisplayZoomReset_Click(object sender, System.EventArgs e)
        {
            bitmapDisplay.ResetZoom();
        }


        private void MenuDisplayActualSize_Click(object sender, System.EventArgs e)
        {
            bitmapDisplay.CentreImageActualSize();
        }

        private void MenuDisplayFitToWindow_Click(object sender, System.EventArgs e)
        {
            bitmapDisplay.FitToWindowCentred();
        }

    }
}
