
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
            bitmapDisplayPanel = new BitmapDisplay.BitmapDisplayPanel();
            panel1 = new System.Windows.Forms.Panel();
            propertyGrid = new System.Windows.Forms.PropertyGrid();
            btnLoadImage = new System.Windows.Forms.Button();
            sysInfoPanel = new Utils.SystemInfoPanel();
            openFileDialog = new System.Windows.Forms.OpenFileDialog();
            multipleROIManager = new RegionOfInterest.MultipleROIManager(components);
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // bitmapDisplayPanel
            // 
            bitmapDisplayPanel.BackgroundImage = Properties.Resources.double_bubble;
            bitmapDisplayPanel.DisplayMode = BitmapDisplay.BitmapDisplayMode.Free;
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
            // openFileDialog
            // 
            openFileDialog.Filter = "JPG files|*.jpg|Bitmap files|*.bmp|TIF files|*.tif|PNG files|*.png";
            // 
            // multipleROIManager
            // 
            multipleROIManager.BitmapDisplayPanel = bitmapDisplayPanel;
            multipleROIManager.CommittedROIRenderer.GrappleDiameter = 6;
            multipleROIManager.CommittedROIRenderer.Locked = false;
            multipleROIManager.CommittedROIRenderer.MaximumSize = new System.Drawing.Size(1000000, 1000000);
            multipleROIManager.CommittedROIRenderer.MinimumSize = new System.Drawing.Size(1, 1);
            multipleROIManager.CommittedROIRenderer.Name = "";
            multipleROIManager.CommittedROIRenderer.PixelAlign = BitmapDisplay.DisplayPixelAlign.TopLeft;
            multipleROIManager.CommittedROIRenderer.Rendering.Fill.Color = System.Drawing.Color.Transparent;
            multipleROIManager.CommittedROIRenderer.Rendering.Font.FontName = "Arial";
            multipleROIManager.CommittedROIRenderer.Rendering.Font.FontSize = 12;
            multipleROIManager.CommittedROIRenderer.Rendering.Lines.Color = System.Drawing.Color.FromArgb(128, 0, 128, 0);
            multipleROIManager.CommittedROIRenderer.Rendering.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            multipleROIManager.CommittedROIRenderer.Rendering.Lines.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManager.CommittedROIRenderer.Rendering.Lines.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManager.CommittedROIRenderer.Rendering.Lines.Width = 2F;
            multipleROIManager.CommittedROIRenderer.Rendering.Visible = true;
            multipleROIManager.CommittedROIRenderer.ROI = new System.Drawing.Rectangle(0, 0, 0, 0);
            multipleROIManager.CommittedROIRenderer.Visible = true;
            multipleROIManager.DraggingROIRenderer.GrappleDiameter = 6;
            multipleROIManager.DraggingROIRenderer.Locked = false;
            multipleROIManager.DraggingROIRenderer.MaximumSize = new System.Drawing.Size(1000000, 1000000);
            multipleROIManager.DraggingROIRenderer.MinimumSize = new System.Drawing.Size(1, 1);
            multipleROIManager.DraggingROIRenderer.Name = "";
            multipleROIManager.DraggingROIRenderer.PixelAlign = BitmapDisplay.DisplayPixelAlign.TopLeft;
            multipleROIManager.DraggingROIRenderer.Rendering.Fill.Color = System.Drawing.Color.Transparent;
            multipleROIManager.DraggingROIRenderer.Rendering.Font.FontName = "Arial";
            multipleROIManager.DraggingROIRenderer.Rendering.Font.FontSize = 12;
            multipleROIManager.DraggingROIRenderer.Rendering.Lines.Color = System.Drawing.Color.FromArgb(128, 255, 165, 0);
            multipleROIManager.DraggingROIRenderer.Rendering.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            multipleROIManager.DraggingROIRenderer.Rendering.Lines.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManager.DraggingROIRenderer.Rendering.Lines.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            multipleROIManager.DraggingROIRenderer.Rendering.Lines.Width = 2F;
            multipleROIManager.DraggingROIRenderer.Rendering.Visible = true;
            multipleROIManager.DraggingROIRenderer.ROI = new System.Drawing.Rectangle(0, 0, 0, 0);
            multipleROIManager.DraggingROIRenderer.Visible = true;
            multipleROIManager.Visible = true;
            multipleROIManager.OnCommittedROIDescriptorChanged += multipleROIManager_OnCommittedROIChanged;
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

        private BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private CDS.Imaging.Utils.SystemInfoPanel sysInfoPanel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private RegionOfInterest.MultipleROIManager multipleROIManager;
    }
}