
namespace CDS.Imaging.Demo.NoCode
{
    partial class FormFitToWindow
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
            this.bitmapDisplayPanel.BackgroundImage = global::CDS.Imaging.Demo.Properties.Resources.double_bubble;
            this.bitmapDisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bitmapDisplayPanel.Image = global::CDS.Imaging.Demo.Properties.Resources.Thailand;
            this.bitmapDisplayPanel.Location = new System.Drawing.Point(0, 0);
            this.bitmapDisplayPanel.DisplayMode = CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayMode.FitToWindowCentred;
            this.bitmapDisplayPanel.Name = "bitmapDisplayPanel";
            this.bitmapDisplayPanel.Size = new System.Drawing.Size(800, 374);
            this.bitmapDisplayPanel.TabIndex = 0;
            this.bitmapDisplayPanel.Zoom = 1.038889F;
            // 
            // FormSimpleFitToWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 374);
            this.Controls.Add(this.bitmapDisplayPanel);
            this.Name = "FormSimpleFitToWindow";
            this.Text = "Simple: fit to window";
            this.ResumeLayout(false);

        }

        #endregion

        private WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel;
    }
}