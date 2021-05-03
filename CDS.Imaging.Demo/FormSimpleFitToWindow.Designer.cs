
namespace CDS.Imaging.Demo
{
    partial class FormSimpleFitToWindow
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
            this.bitmapDisplayPanel = new CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayPanel();
            this.SuspendLayout();
            // 
            // bitmapDisplayPanel
            // 
            this.bitmapDisplayPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bitmapDisplayPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.bitmapDisplayPanel.Location = new System.Drawing.Point(12, 12);
            this.bitmapDisplayPanel.Mode = CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayMode.FitToWindowCentred;
            this.bitmapDisplayPanel.Name = "bitmapDisplayPanel";
            this.bitmapDisplayPanel.Size = new System.Drawing.Size(776, 426);
            this.bitmapDisplayPanel.TabIndex = 0;
            this.bitmapDisplayPanel.Zoom = 1F;
            // 
            // FormSimpleFitToWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.bitmapDisplayPanel);
            this.Name = "FormSimpleFitToWindow";
            this.Text = "FormSimpleFitToWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel;
    }
}