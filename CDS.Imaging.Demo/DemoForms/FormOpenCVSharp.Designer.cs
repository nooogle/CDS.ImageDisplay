namespace CDS.Imaging.Demo.DemoForms
{
    partial class FormOpenCVSharp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.bitmapPanel4 = new CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayPanel();
            this.bitmapPanel3 = new CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayPanel();
            this.bitmapPanel2 = new CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayPanel();
            this.bitmapPanel1 = new CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayPanel();
            this.trackBarGaussianSize = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.trackGaussianSigma = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarGaussianSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackGaussianSigma)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.label5, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.label3, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.bitmapPanel4, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.bitmapPanel3, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.bitmapPanel2, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.bitmapPanel1, 0, 1);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 63);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(776, 375);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 188);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(172, 15);
            this.label6.TabIndex = 7;
            this.label6.Text = "Greyscale OpenCV Mat (8U_C1)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(392, 188);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(212, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "Blurred greyscale OpenCV Mat (8U_C1)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "RGB Bitmap";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(392, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "RGB OpenCV Mat (8U_C3)";
            // 
            // bitmapPanel4
            // 
            this.bitmapPanel4.BackgroundImage = global::CDS.Imaging.Demo.Properties.Resources.double_bubble;
            this.bitmapPanel4.CDSDisplayMode = CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayMode.Free;
            this.bitmapPanel4.CDSZoom = 1F;
            this.bitmapPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bitmapPanel4.Location = new System.Drawing.Point(392, 213);
            this.bitmapPanel4.Name = "bitmapPanel4";
            this.bitmapPanel4.Size = new System.Drawing.Size(379, 157);
            this.bitmapPanel4.TabIndex = 3;
            this.bitmapPanel4.CDSPaintRectChanged += new CDS.Imaging.WinForms.BitmapDisplay.PaintRectChangedEvent(this.bitmapPanel_CDSPaintRectChanged);
            // 
            // bitmapPanel3
            // 
            this.bitmapPanel3.BackgroundImage = global::CDS.Imaging.Demo.Properties.Resources.double_bubble;
            this.bitmapPanel3.CDSDisplayMode = CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayMode.Free;
            this.bitmapPanel3.CDSZoom = 1F;
            this.bitmapPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bitmapPanel3.Location = new System.Drawing.Point(5, 213);
            this.bitmapPanel3.Name = "bitmapPanel3";
            this.bitmapPanel3.Size = new System.Drawing.Size(379, 157);
            this.bitmapPanel3.TabIndex = 2;
            this.bitmapPanel3.CDSPaintRectChanged += new CDS.Imaging.WinForms.BitmapDisplay.PaintRectChangedEvent(this.bitmapPanel_CDSPaintRectChanged);
            // 
            // bitmapPanel2
            // 
            this.bitmapPanel2.BackgroundImage = global::CDS.Imaging.Demo.Properties.Resources.double_bubble;
            this.bitmapPanel2.CDSDisplayMode = CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayMode.Free;
            this.bitmapPanel2.CDSZoom = 1F;
            this.bitmapPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bitmapPanel2.Location = new System.Drawing.Point(392, 27);
            this.bitmapPanel2.Name = "bitmapPanel2";
            this.bitmapPanel2.Size = new System.Drawing.Size(379, 156);
            this.bitmapPanel2.TabIndex = 1;
            this.bitmapPanel2.CDSPaintRectChanged += new CDS.Imaging.WinForms.BitmapDisplay.PaintRectChangedEvent(this.bitmapPanel_CDSPaintRectChanged);
            // 
            // bitmapPanel1
            // 
            this.bitmapPanel1.BackgroundImage = global::CDS.Imaging.Demo.Properties.Resources.double_bubble;
            this.bitmapPanel1.CDSDisplayMode = CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayMode.Free;
            this.bitmapPanel1.CDSZoom = 1F;
            this.bitmapPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bitmapPanel1.Location = new System.Drawing.Point(5, 27);
            this.bitmapPanel1.Name = "bitmapPanel1";
            this.bitmapPanel1.Size = new System.Drawing.Size(379, 156);
            this.bitmapPanel1.TabIndex = 0;
            this.bitmapPanel1.CDSPaintRectChanged += new CDS.Imaging.WinForms.BitmapDisplay.PaintRectChangedEvent(this.bitmapPanel_CDSPaintRectChanged);
            // 
            // trackBarGaussianSize
            // 
            this.trackBarGaussianSize.Location = new System.Drawing.Point(101, 12);
            this.trackBarGaussianSize.Maximum = 30;
            this.trackBarGaussianSize.Name = "trackBarGaussianSize";
            this.trackBarGaussianSize.Size = new System.Drawing.Size(196, 45);
            this.trackBarGaussianSize.TabIndex = 1;
            this.trackBarGaussianSize.Value = 5;
            this.trackBarGaussianSize.Scroll += new System.EventHandler(this.trackBarGaussianSize_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Half kernel size";
            // 
            // trackGaussianSigma
            // 
            this.trackGaussianSigma.Location = new System.Drawing.Point(356, 12);
            this.trackGaussianSigma.Maximum = 30;
            this.trackGaussianSigma.Name = "trackGaussianSigma";
            this.trackGaussianSigma.Size = new System.Drawing.Size(196, 45);
            this.trackGaussianSigma.TabIndex = 3;
            this.trackGaussianSigma.Scroll += new System.EventHandler(this.trackGaussianSigma_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(310, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Sigma";
            // 
            // FormOpenCVSharp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackGaussianSigma);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBarGaussianSize);
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "FormOpenCVSharp";
            this.Text = "FormOpenCVSharp";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarGaussianSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackGaussianSigma)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private WinForms.BitmapDisplay.BitmapDisplayPanel bitmapPanel4;
        private WinForms.BitmapDisplay.BitmapDisplayPanel bitmapPanel3;
        private WinForms.BitmapDisplay.BitmapDisplayPanel bitmapPanel2;
        private WinForms.BitmapDisplay.BitmapDisplayPanel bitmapPanel1;
        private System.Windows.Forms.TrackBar trackBarGaussianSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackGaussianSigma;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}