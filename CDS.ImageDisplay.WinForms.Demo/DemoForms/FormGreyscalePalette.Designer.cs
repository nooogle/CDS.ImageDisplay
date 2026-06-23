namespace CDS.ImageDisplay.WinForms.Demo.DemoForms
{
    partial class FormGreyscalePalette
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _demoBitmap?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            _layout = new System.Windows.Forms.TableLayoutPanel();
            _panelStandard = new CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel();
            _panelInverted = new CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel();
            _panelHighlightSaturated = new CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel();
            _labelStandard = new System.Windows.Forms.Label();
            _labelInverted = new System.Windows.Forms.Label();
            _labelHighlightSaturated = new System.Windows.Forms.Label();
            _layout.SuspendLayout();
            SuspendLayout();
            //
            // _layout
            //
            _layout.ColumnCount = 3;
            _layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33f));
            _layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33f));
            _layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34f));
            _layout.RowCount = 2;
            _layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
            _layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36f));
            _layout.Controls.Add(_panelStandard, 0, 0);
            _layout.Controls.Add(_panelInverted, 1, 0);
            _layout.Controls.Add(_panelHighlightSaturated, 2, 0);
            _layout.Controls.Add(_labelStandard, 0, 1);
            _layout.Controls.Add(_labelInverted, 1, 1);
            _layout.Controls.Add(_labelHighlightSaturated, 2, 1);
            _layout.Dock = System.Windows.Forms.DockStyle.Fill;
            _layout.Name = "_layout";
            //
            // _panelStandard
            //
            _panelStandard.DisplayMode = CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayMode.FitToWindowCentred;
            _panelStandard.GreyscalePaletteMode = CDS.ImageDisplay.WinForms.BitmapDisplay.GreyscalePaletteMode.Standard;
            _panelStandard.Dock = System.Windows.Forms.DockStyle.Fill;
            _panelStandard.Name = "_panelStandard";
            _panelStandard.TabIndex = 0;
            //
            // _panelInverted
            //
            _panelInverted.DisplayMode = CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayMode.FitToWindowCentred;
            _panelInverted.GreyscalePaletteMode = CDS.ImageDisplay.WinForms.BitmapDisplay.GreyscalePaletteMode.Inverted;
            _panelInverted.Dock = System.Windows.Forms.DockStyle.Fill;
            _panelInverted.Name = "_panelInverted";
            _panelInverted.TabIndex = 1;
            //
            // _panelHighlightSaturated
            //
            _panelHighlightSaturated.DisplayMode = CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayMode.FitToWindowCentred;
            _panelHighlightSaturated.GreyscalePaletteMode = CDS.ImageDisplay.WinForms.BitmapDisplay.GreyscalePaletteMode.HighlightSaturated;
            _panelHighlightSaturated.Dock = System.Windows.Forms.DockStyle.Fill;
            _panelHighlightSaturated.Name = "_panelHighlightSaturated";
            _panelHighlightSaturated.TabIndex = 2;
            //
            // _labelStandard
            //
            _labelStandard.Dock = System.Windows.Forms.DockStyle.Fill;
            _labelStandard.Text = "Standard  —  0→black, 255→white";
            _labelStandard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            _labelStandard.Name = "_labelStandard";
            _labelStandard.TabIndex = 3;
            //
            // _labelInverted
            //
            _labelInverted.Dock = System.Windows.Forms.DockStyle.Fill;
            _labelInverted.Text = "Inverted  —  0→white, 255→black";
            _labelInverted.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            _labelInverted.Name = "_labelInverted";
            _labelInverted.TabIndex = 4;
            //
            // _labelHighlightSaturated
            //
            _labelHighlightSaturated.Dock = System.Windows.Forms.DockStyle.Fill;
            _labelHighlightSaturated.Text = "Highlight saturated  —  255→red, rest greyscale";
            _labelHighlightSaturated.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            _labelHighlightSaturated.Name = "_labelHighlightSaturated";
            _labelHighlightSaturated.TabIndex = 5;
            //
            // FormGreyscalePalette
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(960, 400);
            Controls.Add(_layout);
            Name = "FormGreyscalePalette";
            Text = "Greyscale palette modes";
            _layout.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _layout;
        private CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel _panelStandard;
        private CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel _panelInverted;
        private CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel _panelHighlightSaturated;
        private System.Windows.Forms.Label _labelStandard;
        private System.Windows.Forms.Label _labelInverted;
        private System.Windows.Forms.Label _labelHighlightSaturated;
    }
}
