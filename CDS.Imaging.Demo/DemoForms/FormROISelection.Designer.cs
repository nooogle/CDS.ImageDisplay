
namespace CDS.Imaging.Demo.DemoForms
{
    partial class FormROISelection
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
            bitmapDisplayPanel = new WinForms.BitmapDisplay.BitmapDisplayPanel();
            SuspendLayout();
            // 
            // bitmapDisplayPanel
            // 
            bitmapDisplayPanel.CDSDisplayMode = WinForms.BitmapDisplay.BitmapDisplayMode.Free;
            bitmapDisplayPanel.CDSZoom = 1F;
            bitmapDisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapDisplayPanel.Location = new System.Drawing.Point(0, 0);
            bitmapDisplayPanel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            bitmapDisplayPanel.Name = "bitmapDisplayPanel";
            bitmapDisplayPanel.Size = new System.Drawing.Size(1486, 960);
            bitmapDisplayPanel.TabIndex = 0;
            // 
            // FormROISelection
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1486, 960);
            Controls.Add(bitmapDisplayPanel);
            Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            Name = "FormROISelection";
            Text = "ROI selection";
            ResumeLayout(false);
        }

        #endregion

        private WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel;
    }
}