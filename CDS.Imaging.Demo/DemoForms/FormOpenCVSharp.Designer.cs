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
            tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            label6 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            bitmapPanel4 = new WinForms.BitmapDisplay.BitmapDisplayPanel();
            bitmapPanel3 = new WinForms.BitmapDisplay.BitmapDisplayPanel();
            bitmapPanel2 = new WinForms.BitmapDisplay.BitmapDisplayPanel();
            bitmapPanel1 = new WinForms.BitmapDisplay.BitmapDisplayPanel();
            trackBarGaussianSize = new System.Windows.Forms.TrackBar();
            label1 = new System.Windows.Forms.Label();
            trackGaussianSigma = new System.Windows.Forms.TrackBar();
            label2 = new System.Windows.Forms.Label();
            labelProcessTimeMS = new System.Windows.Forms.Label();
            tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarGaussianSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackGaussianSigma).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel.Controls.Add(label6, 0, 2);
            tableLayoutPanel.Controls.Add(label5, 1, 2);
            tableLayoutPanel.Controls.Add(label4, 0, 0);
            tableLayoutPanel.Controls.Add(label3, 1, 0);
            tableLayoutPanel.Controls.Add(bitmapPanel4, 1, 3);
            tableLayoutPanel.Controls.Add(bitmapPanel3, 0, 3);
            tableLayoutPanel.Controls.Add(bitmapPanel2, 1, 1);
            tableLayoutPanel.Controls.Add(bitmapPanel1, 0, 1);
            tableLayoutPanel.Location = new System.Drawing.Point(22, 134);
            tableLayoutPanel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 4;
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel.Size = new System.Drawing.Size(1441, 800);
            tableLayoutPanel.TabIndex = 0;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(8, 401);
            label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(348, 32);
            label6.TabIndex = 7;
            label6.Text = "Greyscale OpenCV Mat (8U_C1)";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(727, 401);
            label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(430, 32);
            label5.TabIndex = 6;
            label5.Text = "Blurred greyscale OpenCV Mat (8U_C1)";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(8, 2);
            label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(140, 32);
            label4.TabIndex = 5;
            label4.Text = "RGB Bitmap";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(727, 2);
            label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(291, 32);
            label3.TabIndex = 4;
            label3.Text = "RGB OpenCV Mat (8U_C3)";
            // 
            // bitmapPanel4
            // 
            bitmapPanel4.BackgroundImage = Properties.Resources.double_bubble;
            bitmapPanel4.CDSDisplayMode = WinForms.BitmapDisplay.BitmapDisplayMode.Free;
            bitmapPanel4.CDSZoom = 1F;
            bitmapPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapPanel4.Location = new System.Drawing.Point(727, 452);
            bitmapPanel4.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            bitmapPanel4.Name = "bitmapPanel4";
            bitmapPanel4.Size = new System.Drawing.Size(706, 340);
            bitmapPanel4.TabIndex = 3;
            bitmapPanel4.CDSPaintRectChanged += bitmapPanel_CDSPaintRectChanged;
            // 
            // bitmapPanel3
            // 
            bitmapPanel3.BackgroundImage = Properties.Resources.double_bubble;
            bitmapPanel3.CDSDisplayMode = WinForms.BitmapDisplay.BitmapDisplayMode.Free;
            bitmapPanel3.CDSZoom = 1F;
            bitmapPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapPanel3.Location = new System.Drawing.Point(8, 452);
            bitmapPanel3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            bitmapPanel3.Name = "bitmapPanel3";
            bitmapPanel3.Size = new System.Drawing.Size(705, 340);
            bitmapPanel3.TabIndex = 2;
            bitmapPanel3.CDSPaintRectChanged += bitmapPanel_CDSPaintRectChanged;
            // 
            // bitmapPanel2
            // 
            bitmapPanel2.BackgroundImage = Properties.Resources.double_bubble;
            bitmapPanel2.CDSDisplayMode = WinForms.BitmapDisplay.BitmapDisplayMode.Free;
            bitmapPanel2.CDSZoom = 1F;
            bitmapPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapPanel2.Location = new System.Drawing.Point(727, 53);
            bitmapPanel2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            bitmapPanel2.Name = "bitmapPanel2";
            bitmapPanel2.Size = new System.Drawing.Size(706, 340);
            bitmapPanel2.TabIndex = 1;
            bitmapPanel2.CDSPaintRectChanged += bitmapPanel_CDSPaintRectChanged;
            // 
            // bitmapPanel1
            // 
            bitmapPanel1.BackgroundImage = Properties.Resources.double_bubble;
            bitmapPanel1.CDSDisplayMode = WinForms.BitmapDisplay.BitmapDisplayMode.Free;
            bitmapPanel1.CDSZoom = 1F;
            bitmapPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapPanel1.Location = new System.Drawing.Point(8, 53);
            bitmapPanel1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            bitmapPanel1.Name = "bitmapPanel1";
            bitmapPanel1.Size = new System.Drawing.Size(705, 340);
            bitmapPanel1.TabIndex = 0;
            bitmapPanel1.CDSPaintRectChanged += bitmapPanel_CDSPaintRectChanged;
            // 
            // trackBarGaussianSize
            // 
            trackBarGaussianSize.Location = new System.Drawing.Point(188, 26);
            trackBarGaussianSize.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            trackBarGaussianSize.Maximum = 30;
            trackBarGaussianSize.Name = "trackBarGaussianSize";
            trackBarGaussianSize.Size = new System.Drawing.Size(364, 90);
            trackBarGaussianSize.TabIndex = 1;
            trackBarGaussianSize.Value = 5;
            trackBarGaussianSize.Scroll += trackBarGaussianSize_Scroll;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(22, 26);
            label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(177, 32);
            label1.TabIndex = 2;
            label1.Text = "Half kernel size";
            // 
            // trackGaussianSigma
            // 
            trackGaussianSigma.Location = new System.Drawing.Point(661, 26);
            trackGaussianSigma.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            trackGaussianSigma.Maximum = 30;
            trackGaussianSigma.Name = "trackGaussianSigma";
            trackGaussianSigma.Size = new System.Drawing.Size(364, 90);
            trackGaussianSigma.TabIndex = 3;
            trackGaussianSigma.Scroll += trackGaussianSigma_Scroll;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(576, 26);
            label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(80, 32);
            label2.TabIndex = 4;
            label2.Text = "Sigma";
            // 
            // labelProcessTimeMS
            // 
            labelProcessTimeMS.AutoSize = true;
            labelProcessTimeMS.Location = new System.Drawing.Point(1043, 26);
            labelProcessTimeMS.Name = "labelProcessTimeMS";
            labelProcessTimeMS.Size = new System.Drawing.Size(204, 32);
            labelProcessTimeMS.TabIndex = 5;
            labelProcessTimeMS.Text = "Process time: 0ms";
            // 
            // FormOpenCVSharp
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1486, 960);
            Controls.Add(labelProcessTimeMS);
            Controls.Add(label2);
            Controls.Add(trackGaussianSigma);
            Controls.Add(label1);
            Controls.Add(trackBarGaussianSize);
            Controls.Add(tableLayoutPanel);
            Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            Name = "FormOpenCVSharp";
            Text = "FormOpenCVSharp";
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarGaussianSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackGaussianSigma).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.Label labelProcessTimeMS;
    }
}