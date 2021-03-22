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
            msgPanelBrush = new SolidBrush(Color.FromArgb(64, Color.Yellow));
        }

        private void FormBitmapDisplay_Load(object sender, EventArgs e)
        {
            bitmapDisplay.SetImage(Properties.Resources.Budapest_8U1C);
            bitmapDisplay.CDS.Mode = WinForms.BitmapDisplayMode.Free;
        }

        private void bitmapDisplay_PaintOver(WinForms.BitmapDisplay sender, Graphics graphics, Size imageSize, RectangleF renderRect)
        {
            var info = new StringBuilder();
            info.Append($"Display mode      {sender.CDS.Mode}\n");
            info.Append($"Size              {sender.CDS.Image.Size}\n");
            info.Append($"Format            {sender.CDS.Image.PixelFormat}\n");
            info.Append($"Set image         {sender.CDS.TimingMetrics.SetImage.TotalMilliseconds:0.0}ms\n");
            info.Append($"Paint foreground  {sender.CDS.TimingMetrics.ForegroundPaint.TotalMilliseconds:0.0}ms\n");
            info.Append($"Paint background  {sender.CDS.TimingMetrics.BackgroundPaint.TotalMilliseconds:0.0}ms\n");

            var textTopleft = new PointF(panelControlBox.Left, panelControlBox.Bottom + 10);
            var textBlockSize = graphics.MeasureString(info.ToString(), fixedWidthFont);

            var bkRect = new RectangleF(
                textTopleft,
                textBlockSize);

            graphics.FillRectangle(msgPanelBrush, bkRect);
            graphics.DrawRectangle(Pens.Navy, Rectangle.Round(bkRect));

            graphics.DrawString(
                info.ToString(), 
                fixedWidthFont, 
                Brushes.Navy, 
                textTopleft);

            var topLeftBox = sender.DisplayCoordsFromImage(new RectangleF(0, 0, 10, 5)).Value;
            graphics.DrawRectangle(Pens.Red, topLeftBox.X, topLeftBox.Y, topLeftBox.Width, topLeftBox.Height);

            graphics.DrawString(
                "A 10*5 box",
                fixedWidthFont,
                Brushes.Navy,
                new PointF(topLeftBox.X, topLeftBox.Y));
        }

        private void rbtnDisplayModeFree_CheckedChanged(object sender, EventArgs e)
        {
            if(rbtnDisplayModeFree.Checked)
            {
                bitmapDisplay.CDS.Mode = WinForms.BitmapDisplayMode.Free;
                UpdateCommandEnablement();
            }
        }

        private void UpdateCommandEnablement()
        {
        }

        private void btnFitToWindow_Click(object sender, EventArgs e)
        {
            rbtnDisplayModeFree.Checked = true;
            bitmapDisplay.CDS.FitToWindowCentred();
        }


        private void btnActualSize_Click(object sender, EventArgs e)
        {
            rbtnDisplayModeFree.Checked = true;
            bitmapDisplay.CDS.ActualSizeCentred();
        }

        private void rbtnDisplayModeFitToWindow_CheckedChanged(object sender, EventArgs e)
        {
            if(rbtnDisplayModeFitToWindow.Checked)
            {
                bitmapDisplay.CDS.Mode = WinForms.BitmapDisplayMode.FitToWindowCentred;
                UpdateCommandEnablement();
            }
        }

        private void rbtnDisplayModeActualSize_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnDisplayModeActualSize.Checked)
            {
                bitmapDisplay.CDS.Mode = WinForms.BitmapDisplayMode.ActualSizeCentred;
                UpdateCommandEnablement();
            }
        }

        private void rbtnDisplayModeLocked_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnDisplayModeLocked.Checked)
            {
                bitmapDisplay.CDS.Mode = WinForms.BitmapDisplayMode.Locked;
                UpdateCommandEnablement();
            }
        }

        private void btnCentre_Click(object sender, EventArgs e)
        {
            rbtnDisplayModeFree.Checked = true;
            bitmapDisplay.CDS.Centre();
        }
    }
}
