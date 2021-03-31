using Humanizer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CDS.Imaging.Demo
{
    public partial class FormBitmapDisplay : Form
    {
        Font fixedWidthFont;
        Brush msgPanelBrush;

        public FormBitmapDisplay()
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


        private void bitmapDisplay_PaintOver(WinForms.BitmapDisplay sender, Graphics graphics, Size imageSize, RectangleF renderRect)
        {
            var info = new StringBuilder();
            info.Append($"Display mode      {sender.CDS.Mode.Humanize()}\n");
            info.Append($"Display size      {sender.ClientSize}\n");
            if (!sender.CDS.AnythingToDisplay)
            {
                info.Append($"Image not loaded\n");
            }
            else
            {
                var r = sender.CDS.PaintRect;
                info.Append($"Bitmap size       {sender.CDS.Image.Size}\n");
                info.Append($"Paint rect        {r.X:0.0}, {r.Y:0.0}, {r.Width:0.0}, {r.Height:0:0}\n");
                info.Append($"Format            {sender.CDS.Image.PixelFormat.Humanize()}\n");
            }
            info.Append($"Set image         {sender.CDS.TimingMetrics.SetImage.Humanize()}\n");
            info.Append($"Paint foreground  {sender.CDS.TimingMetrics.ForegroundPaint.Humanize()}\n");
            info.Append($"Paint background  {sender.CDS.TimingMetrics.BackgroundPaint.Humanize()}\n");

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

            var topLeftBox = sender.CDS.MapImageToDisplay(new RectangleF(0, 0, 10, 5));
            graphics.DrawRectangle(Pens.Red, topLeftBox.X, topLeftBox.Y, topLeftBox.Width, topLeftBox.Height);

            graphics.DrawString(
                "A 10*5 box",
                fixedWidthFont,
                Brushes.Navy,
                new PointF(topLeftBox.X, topLeftBox.Y));

            crossHair1.Draw(
                graphics,
                bitmapDisplay.CDS.MapImageToDisplay(bitmapDisplay.CDS.ImageDisplayCentre));
        }

        private void menuDisplayModeFree_Click(object sender, EventArgs e)
        {
            bitmapDisplay.CDS.Mode = WinForms.BitmapDisplayMode.Free;
            UpdateCommandEnablement();
            bitmapDisplay.Invalidate();
        }

        private void UpdateCommandEnablement()
        {
            menuDisplayCentre.Enabled = (bitmapDisplay.CDS.Mode == WinForms.BitmapDisplayMode.Free);
            menuDisplayZoomIn.Enabled = (bitmapDisplay.CDS.Mode == WinForms.BitmapDisplayMode.Free);
            menuDisplayZoomOut.Enabled = (bitmapDisplay.CDS.Mode == WinForms.BitmapDisplayMode.Free);
            menuDisplayZoomReset.Enabled = (bitmapDisplay.CDS.Mode == WinForms.BitmapDisplayMode.Free);
        }

        private void menuDisplayModeFitToWindow_Click(object sender, EventArgs e)
        {
            bitmapDisplay.CDS.Mode = WinForms.BitmapDisplayMode.FitToWindowCentred;
            UpdateCommandEnablement();
            bitmapDisplay.Invalidate();
        }

        private void menuDisplayModeActualSize_Click(object sender, EventArgs e)
        {
            bitmapDisplay.CDS.Mode = WinForms.BitmapDisplayMode.ActualSizeCentred;
            UpdateCommandEnablement();
            bitmapDisplay.Invalidate();
        }

        private void menuDisplayModeLocked_Click(object sender, EventArgs e)
        {
            bitmapDisplay.CDS.Mode = WinForms.BitmapDisplayMode.Locked;
            UpdateCommandEnablement();
            bitmapDisplay.Invalidate();
        }

        private void menuDisplayCentre_Click(object sender, EventArgs e)
        {
            bitmapDisplay.CDS.Centre();
        }


        private void MenuImageBuiltIn_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var imageNameAndResource = menuImageBuiltIn.SelectedItem as ImageNameAndResource;
            bitmapDisplay.SetImage(imageNameAndResource.Bitmap);
        }


        private void bitmapDisplay_PaintUnder(WinForms.BitmapDisplay sender, Graphics graphics, Size imageSize, RectangleF renderRect)
        {
            if(renderRect.IsEmpty) { return; }

            graphics.DrawLine(
                Pens.Navy, 
                renderRect.Location, 
                new PointF(renderRect.Right - 1, renderRect.Bottom - 1));

            graphics.DrawLine(
                Pens.Navy, 
                new PointF(renderRect.Right - 1, renderRect.Top), 
                new PointF(renderRect.Left, renderRect.Bottom - 1));
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
                using var bitmap = (Bitmap)Image.FromFile(fileName);
                bitmapDisplay.SetImage(bitmap);
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

        private void bitmapDisplay_DisplayModeChanged(WinForms.BitmapDisplay sender, WinForms.BitmapDisplayModeEventArgs modeChangedArgs)
        {
            UpdateCommandEnablement();
            UpdateDisplayModeCheckboxes();
        }


        public void UpdateDisplayModeCheckboxes()
        {
            menuDisplayModeFitToWindow.Checked = (bitmapDisplay.Mode == WinForms.BitmapDisplayMode.FitToWindowCentred);
            menuDisplayModeActualSize.Checked = (bitmapDisplay.Mode == WinForms.BitmapDisplayMode.ActualSizeCentred);
            menuDisplayModeFree.Checked = (bitmapDisplay.Mode == WinForms.BitmapDisplayMode.Free);
            menuDisplayModeLocked.Checked = (bitmapDisplay.Mode == WinForms.BitmapDisplayMode.Locked);
        }

        private void menuDisplayZoomOut_Click(object sender, System.EventArgs e)
        {
            bitmapDisplay.CDS.ZoomOut();
        }

        private void menuDisplayZoomIn_Click(object sender, System.EventArgs e)
        {
            bitmapDisplay.CDS.ZoomIn();
        }

        private void menuDisplayZoomReset_Click(object sender, System.EventArgs e)
        {
            bitmapDisplay.CDS.ResetZoom();
        }
    }
}
