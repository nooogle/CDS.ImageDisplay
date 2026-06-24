namespace CDS.ImageDisplay.WinForms.Demo.DemoForms
{
    partial class FormScaleFactorDemo
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            _layout = new System.Windows.Forms.TableLayoutPanel();
            _labelDescription = new System.Windows.Forms.Label();
            _panelFull = new CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel();
            _panelQuarter = new CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel();
            _labelFull = new System.Windows.Forms.Label();
            _labelQuarter = new System.Windows.Forms.Label();
            _layout.SuspendLayout();
            SuspendLayout();
            //
            // _layout
            //
            _layout.ColumnCount = 2;
            _layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
            _layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
            _layout.RowCount = 3;
            _layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56f));
            _layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
            _layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36f));
            _layout.Controls.Add(_labelDescription, 0, 0);
            _layout.Controls.Add(_panelFull, 0, 1);
            _layout.Controls.Add(_panelQuarter, 1, 1);
            _layout.Controls.Add(_labelFull, 0, 2);
            _layout.Controls.Add(_labelQuarter, 1, 2);
            _layout.Dock = System.Windows.Forms.DockStyle.Fill;
            _layout.Name = "_layout";
            _layout.SetColumnSpan(_labelDescription, 2);
            //
            // _labelDescription
            //
            _labelDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            _labelDescription.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            _labelDescription.Name = "_labelDescription";
            _labelDescription.TabIndex = 0;
            _labelDescription.Text =
                "The yellow rectangle overlay is defined once in full-size image coordinates (160, 130, 220×250). " +
                "The right panel shows the same image at quarter size with MapImageToDisplayScaleFactor = 0.25, " +
                "so the overlay correctly encloses the same blobs on both panels. Pan and zoom each panel independently.";
            //
            // _panelFull
            //
            _panelFull.DisplayMode = CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayMode.Free;
            _panelFull.Dock = System.Windows.Forms.DockStyle.Fill;
            _panelFull.Name = "_panelFull";
            _panelFull.TabIndex = 1;
            _panelFull.Zoom = 1F;
            _panelFull.PaintOver += PanelFull_PaintOver;
            //
            // _panelQuarter
            //
            _panelQuarter.DisplayMode = CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayMode.Free;
            _panelQuarter.Dock = System.Windows.Forms.DockStyle.Fill;
            _panelQuarter.Name = "_panelQuarter";
            _panelQuarter.TabIndex = 2;
            _panelQuarter.Zoom = 1F;
            _panelQuarter.PaintOver += PanelQuarter_PaintOver;
            //
            // _labelFull
            //
            _labelFull.Dock = System.Windows.Forms.DockStyle.Fill;
            _labelFull.Text = "Full size (640 × 480)";
            _labelFull.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            _labelFull.Name = "_labelFull";
            _labelFull.TabIndex = 3;
            //
            // _labelQuarter
            //
            _labelQuarter.Dock = System.Windows.Forms.DockStyle.Fill;
            _labelQuarter.Text = "Quarter size (160 × 120)  —  MapImageToDisplayScaleFactor = 0.25";
            _labelQuarter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            _labelQuarter.Name = "_labelQuarter";
            _labelQuarter.TabIndex = 4;
            //
            // FormScaleFactorDemo
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1100, 580);
            Controls.Add(_layout);
            Name = "FormScaleFactorDemo";
            Text = "Overlay scale factor";
            _layout.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _layout;
        private System.Windows.Forms.Label _labelDescription;
        private CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel _panelFull;
        private CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel _panelQuarter;
        private System.Windows.Forms.Label _labelFull;
        private System.Windows.Forms.Label _labelQuarter;
    }
}
