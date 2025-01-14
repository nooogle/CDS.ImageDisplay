
namespace CDS.Imaging.Demo.DemoForms.MultipleROIs
{
    partial class FormMultipleROIs
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
            bitmapDisplayPanel = new WinForms.BitmapDisplay.BitmapDisplayPanel();
            panel1 = new System.Windows.Forms.Panel();
            propertyGrid = new System.Windows.Forms.PropertyGrid();
            btnLoadImage = new System.Windows.Forms.Button();
            sysInfoPanel = new WinForms.SysInfoPanel();
            multipleROIManagerOnBitmapDisplay = new WinForms.RegionOfInterest.MultipleROIManager(components);
            openFileDialog = new System.Windows.Forms.OpenFileDialog();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // bitmapDisplayPanel
            // 
            bitmapDisplayPanel.BackgroundImage = Properties.Resources.double_bubble;
            bitmapDisplayPanel.DisplayMode = WinForms.BitmapDisplay.BitmapDisplayMode.Free;
            bitmapDisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapDisplayPanel.Location = new System.Drawing.Point(448, 38);
            bitmapDisplayPanel.Name = "bitmapDisplayPanel";
            bitmapDisplayPanel.Size = new System.Drawing.Size(890, 640);
            bitmapDisplayPanel.TabIndex = 0;
            bitmapDisplayPanel.Zoom = 1F;
            // 
            // panel1
            // 
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel1.Controls.Add(propertyGrid);
            panel1.Controls.Add(btnLoadImage);
            panel1.Dock = System.Windows.Forms.DockStyle.Left;
            panel1.Location = new System.Drawing.Point(0, 38);
            panel1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(448, 640);
            panel1.TabIndex = 2;
            // 
            // propertyGrid
            // 
            propertyGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            propertyGrid.Location = new System.Drawing.Point(11, 52);
            propertyGrid.Name = "propertyGrid";
            propertyGrid.Size = new System.Drawing.Size(424, 575);
            propertyGrid.TabIndex = 6;
            propertyGrid.PropertyValueChanged += propertyGrid_PropertyValueChanged;
            // 
            // btnLoadImage
            // 
            btnLoadImage.Location = new System.Drawing.Point(11, 11);
            btnLoadImage.Name = "btnLoadImage";
            btnLoadImage.Size = new System.Drawing.Size(124, 23);
            btnLoadImage.TabIndex = 2;
            btnLoadImage.Text = "Load image";
            btnLoadImage.UseVisualStyleBackColor = true;
            btnLoadImage.Click += btnLoadImage_Click;
            // 
            // sysInfoPanel
            // 
            sysInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            sysInfoPanel.Location = new System.Drawing.Point(0, 0);
            sysInfoPanel.Name = "sysInfoPanel";
            sysInfoPanel.Size = new System.Drawing.Size(1338, 38);
            sysInfoPanel.TabIndex = 7;
            // 
            // multipleROIManagerOnBitmapDisplay
            // 
            multipleROIManagerOnBitmapDisplay.BitmapDisplayPanel = bitmapDisplayPanel;
            // 
            // 
            // 
            // 
            // 
            // 
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.DisabledGrappleBrush.Color = System.Drawing.Color.Gray;
            // 
            // 
            // 
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.DisabledGrapplePen.Color = System.Drawing.Color.Gray;
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.DisabledGrapplePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.DisabledGrapplePen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.DisabledGrapplePen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.DisabledGrapplePen.Width = 1F;
            // 
            // 
            // 
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.EnabledGrappleBrush.Color = System.Drawing.Color.Navy;
            // 
            // 
            // 
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.EnabledGrapplePen.Color = System.Drawing.Color.Navy;
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.EnabledGrapplePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.EnabledGrapplePen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.EnabledGrapplePen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.EnabledGrapplePen.Width = 1F;
            // 
            // 
            // 
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.FillBrush.Color = System.Drawing.Color.Transparent;
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.GrappleDiameter = 6;
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.GrapplesMode = WinForms.RegionOfInterest.RectangleRenderer.GrapplesRenderingMode.ShowEnabled;
            // 
            // 
            // 
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.OutlinePen.Color = System.Drawing.Color.FromArgb(128, 255, 0, 0);
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.OutlinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.OutlinePen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.OutlinePen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.OutlinePen.Width = 2F;
            multipleROIManagerOnBitmapDisplay.CommittedROIRenderer.Visible = true;
            // 
            // 
            // 
            // 
            // 
            // 
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.DisabledGrappleBrush.Color = System.Drawing.Color.Gray;
            // 
            // 
            // 
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.DisabledGrapplePen.Color = System.Drawing.Color.Gray;
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.DisabledGrapplePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.DisabledGrapplePen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.DisabledGrapplePen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.DisabledGrapplePen.Width = 2F;
            // 
            // 
            // 
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.EnabledGrappleBrush.Color = System.Drawing.Color.FromArgb(128, 0, 0, 128);
            // 
            // 
            // 
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.EnabledGrapplePen.Color = System.Drawing.Color.FromArgb(128, 0, 255, 255);
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.EnabledGrapplePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.EnabledGrapplePen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.EnabledGrapplePen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.EnabledGrapplePen.Width = 2F;
            // 
            // 
            // 
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.FillBrush.Color = System.Drawing.Color.FromArgb(32, 255, 0, 0);
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.GrappleDiameter = 6;
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.GrapplesMode = WinForms.RegionOfInterest.RectangleRenderer.GrapplesRenderingMode.ShowEnabled;
            // 
            // 
            // 
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.OutlinePen.Color = System.Drawing.Color.Lime;
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.OutlinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.OutlinePen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.OutlinePen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.OutlinePen.Width = 10F;
            multipleROIManagerOnBitmapDisplay.DraggingROIRenderer.Visible = true;
            multipleROIManagerOnBitmapDisplay.Visible = true;
            // 
            // openFileDialog
            // 
            openFileDialog.Filter = "JPG files|*.jpg|Bitmap files|*.bmp|TIF files|*.tif|PNG files|*.png";
            // 
            // FormMultipleROIs
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1338, 678);
            Controls.Add(bitmapDisplayPanel);
            Controls.Add(panel1);
            Controls.Add(sysInfoPanel);
            Name = "FormMultipleROIs";
            Text = "Multiple ROIs";
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private CDS.Imaging.WinForms.SysInfoPanel sysInfoPanel;
        private WinForms.RegionOfInterest.MultipleROIManager multipleROIManagerOnBitmapDisplay;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}