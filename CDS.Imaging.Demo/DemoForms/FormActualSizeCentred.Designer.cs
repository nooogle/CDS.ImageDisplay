
namespace CDS.Imaging.Demo.DemoForms
{
    partial class FormActualSizeCentred
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
            this.bitmapDisplayPanel1 = new CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayPanel();
            this.SuspendLayout();
            // 
            // bitmapDisplayPanel1
            // 
            this.bitmapDisplayPanel1.BackgroundImage = global::CDS.Imaging.Demo.Properties.Resources.double_bubble;
            this.bitmapDisplayPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bitmapDisplayPanel1.Location = new System.Drawing.Point(0, 0);
            this.bitmapDisplayPanel1.DisplayMode = CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayMode.ActualSizeCentred;
            this.bitmapDisplayPanel1.Name = "bitmapDisplayPanel1";
            this.bitmapDisplayPanel1.Size = new System.Drawing.Size(800, 450);
            this.bitmapDisplayPanel1.TabIndex = 0;
            this.bitmapDisplayPanel1.Zoom = 1F;
            // 
            // FormSimpleActualSizeCentred
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.bitmapDisplayPanel1);
            this.Name = "FormSimpleActualSizeCentred";
            this.Text = "Simple: actual size centered";
            this.ResumeLayout(false);

        }

        #endregion

        private WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel1;
    }
}