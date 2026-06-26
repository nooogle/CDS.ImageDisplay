namespace CDS.ImageDisplay.WinForms.Demo.DemoForms.AnnotationsDemo
{
    partial class FormAnnotationsToBitmap
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
            components = new System.ComponentModel.Container();
            bitmapDisplayPanel = new CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel();
            sysInfoPanel = new CDS.ImageDisplay.WinForms.Utils.SystemInfoPanel();
            btnSavePng = new System.Windows.Forms.Button();
            labelStatus = new System.Windows.Forms.Label();
            annotationManager = new CDS.ImageDisplay.WinForms.Annotations.AnnotationManager(components);
            SuspendLayout();
            //
            // bitmapDisplayPanel
            //
            bitmapDisplayPanel.DisplayMode = BitmapDisplay.BitmapDisplayMode.Free;
            bitmapDisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapDisplayPanel.Location = new System.Drawing.Point(0, 38);
            bitmapDisplayPanel.Name = "bitmapDisplayPanel";
            bitmapDisplayPanel.Size = new System.Drawing.Size(800, 350);
            bitmapDisplayPanel.TabIndex = 0;
            //
            // sysInfoPanel
            //
            sysInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            sysInfoPanel.Location = new System.Drawing.Point(0, 0);
            sysInfoPanel.Name = "sysInfoPanel";
            sysInfoPanel.Size = new System.Drawing.Size(800, 38);
            sysInfoPanel.TabIndex = 1;
            //
            // btnSavePng
            //
            btnSavePng.Dock = System.Windows.Forms.DockStyle.Bottom;
            btnSavePng.Location = new System.Drawing.Point(0, 388);
            btnSavePng.Name = "btnSavePng";
            btnSavePng.Size = new System.Drawing.Size(800, 38);
            btnSavePng.TabIndex = 3;
            btnSavePng.Text = "Save to PNG...";
            btnSavePng.Click += btnSavePng_Click;
            //
            // labelStatus
            //
            labelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            labelStatus.Location = new System.Drawing.Point(0, 426);
            labelStatus.Name = "labelStatus";
            labelStatus.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            labelStatus.Size = new System.Drawing.Size(800, 24);
            labelStatus.TabIndex = 2;
            labelStatus.Text = "Count: 0 | Last: <none>";
            //
            // annotationManager
            //
            annotationManager.BitmapDisplayPanel = bitmapDisplayPanel;
            annotationManager.AnnotationCreated += annotationManager_AnnotationCreated;
            annotationManager.AnnotationDeleted += annotationManager_AnnotationDeleted;
            //
            // FormAnnotationsToBitmap
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(bitmapDisplayPanel);
            Controls.Add(sysInfoPanel);
            Controls.Add(btnSavePng);
            Controls.Add(labelStatus);
            Name = "FormAnnotationsToBitmap";
            Text = "Annotations: save to bitmap";
            ResumeLayout(false);
        }

        #endregion

        private WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel;
        private WinForms.Utils.SystemInfoPanel sysInfoPanel;
        private System.Windows.Forms.Button btnSavePng;
        private System.Windows.Forms.Label labelStatus;
        private WinForms.Annotations.AnnotationManager annotationManager;
    }
}
