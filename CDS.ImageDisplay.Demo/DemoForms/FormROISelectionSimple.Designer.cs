
namespace CDS.ImageDisplay.Demo.DemoForms
{
    partial class FormROISelectionSimple
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
            bitmapDisplayPanel = new CDS.ImageDisplay.BitmapDisplay.BitmapDisplayPanel();
            sysInfoPanel = new CDS.ImageDisplay.Utils.SystemInfoPanel();
            singleROIManager = new CDS.ImageDisplay.RegionOfInterest.SingleROIManager(components);
            SuspendLayout();
            // 
            // bitmapDisplayPanel
            // 
            bitmapDisplayPanel.BackgroundImage = Properties.Resources.double_bubble;
            bitmapDisplayPanel.DisplayMode = BitmapDisplay.BitmapDisplayMode.Free;
            bitmapDisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapDisplayPanel.Location = new System.Drawing.Point(0, 38);
            bitmapDisplayPanel.Name = "bitmapDisplayPanel";
            bitmapDisplayPanel.Size = new System.Drawing.Size(800, 412);
            bitmapDisplayPanel.TabIndex = 0;
            bitmapDisplayPanel.OnPaintOver += bitmapDisplayPanel_OnPaintOver;
            // 
            // sysInfoPanel
            // 
            sysInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            sysInfoPanel.Location = new System.Drawing.Point(0, 0);
            sysInfoPanel.Name = "sysInfoPanel";
            sysInfoPanel.Size = new System.Drawing.Size(800, 38);
            sysInfoPanel.TabIndex = 7;
            // 
            // singleROIManager
            // 
            singleROIManager.BitmapDisplayPanel = bitmapDisplayPanel;
            singleROIManager.CommittedROIDrawingSpec.Fill.Color = System.Drawing.Color.Transparent;
            singleROIManager.CommittedROIDrawingSpec.Font.FontName = "Arial";
            singleROIManager.CommittedROIDrawingSpec.Font.FontSize = 12;
            singleROIManager.CommittedROIDrawingSpec.Font.FontStyle = System.Drawing.FontStyle.Regular;
            singleROIManager.CommittedROIDrawingSpec.Lines.Color = System.Drawing.Color.FromArgb(128, 0, 128, 0);
            singleROIManager.CommittedROIDrawingSpec.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            singleROIManager.CommittedROIDrawingSpec.Lines.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            singleROIManager.CommittedROIDrawingSpec.Lines.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            singleROIManager.CommittedROIDrawingSpec.Lines.Width = 2F;
            singleROIManager.CommittedROIDrawingSpec.MappingMode = Overlays.MappingMode.ImageToDisplay;
            singleROIManager.CommittedROIDrawingSpec.Visible = true;
            singleROIManager.CommittedROIShape.Drawing = singleROIManager.CommittedROIDrawingSpec;
            singleROIManager.CommittedROIShape.GrappleDiameter = 6;
            singleROIManager.CommittedROIShape.GrapplesVisible = true;
            singleROIManager.CommittedROIShape.Locked = false;
            singleROIManager.CommittedROIShape.MaximumSize = new System.Drawing.Size(1000000, 1000000);
            singleROIManager.CommittedROIShape.MinimumSize = new System.Drawing.Size(1, 1);
            singleROIManager.CommittedROIShape.Name = "";
            singleROIManager.CommittedROIShape.PixelAlign = BitmapDisplay.DisplayPixelAlign.TopLeft;
            singleROIManager.CommittedROIShape.ROI = new System.Drawing.Rectangle(0, 0, 0, 0);
            singleROIManager.CommittedROIShape.Visible = true;
            singleROIManager.LiveDraggingROIDrawingSpec.Fill.Color = System.Drawing.Color.FromArgb(32, 255, 128, 0);
            singleROIManager.LiveDraggingROIDrawingSpec.Font.FontName = "Arial";
            singleROIManager.LiveDraggingROIDrawingSpec.Font.FontSize = 12;
            singleROIManager.LiveDraggingROIDrawingSpec.Font.FontStyle = System.Drawing.FontStyle.Regular;
            singleROIManager.LiveDraggingROIDrawingSpec.Lines.Color = System.Drawing.Color.FromArgb(192, 255, 128, 0);
            singleROIManager.LiveDraggingROIDrawingSpec.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            singleROIManager.LiveDraggingROIDrawingSpec.Lines.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            singleROIManager.LiveDraggingROIDrawingSpec.Lines.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            singleROIManager.LiveDraggingROIDrawingSpec.Lines.Width = 2F;
            singleROIManager.LiveDraggingROIDrawingSpec.MappingMode = Overlays.MappingMode.ImageToDisplay;
            singleROIManager.LiveDraggingROIDrawingSpec.Visible = true;
            singleROIManager.LiveDraggingROIShape.Drawing = singleROIManager.LiveDraggingROIDrawingSpec;
            singleROIManager.LiveDraggingROIShape.GrappleDiameter = 6;
            singleROIManager.LiveDraggingROIShape.GrapplesVisible = true;
            singleROIManager.LiveDraggingROIShape.Locked = false;
            singleROIManager.LiveDraggingROIShape.MaximumSize = new System.Drawing.Size(1000000, 1000000);
            singleROIManager.LiveDraggingROIShape.MinimumSize = new System.Drawing.Size(1, 1);
            singleROIManager.LiveDraggingROIShape.Name = "";
            singleROIManager.LiveDraggingROIShape.PixelAlign = BitmapDisplay.DisplayPixelAlign.TopLeft;
            singleROIManager.LiveDraggingROIShape.ROI = new System.Drawing.Rectangle(0, 0, 0, 0);
            singleROIManager.LiveDraggingROIShape.Visible = true;
            singleROIManager.OnCommittedROIChanged += singleROIManager_OnCommittedROIChanged;
            singleROIManager.OnDraggingROIChanged += singleROIManager_OnDraggingROIChanged;
            // 
            // FormROISelectionSimple
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(bitmapDisplayPanel);
            Controls.Add(sysInfoPanel);
            Name = "FormROISelectionSimple";
            Text = "ROI selection";
            ResumeLayout(false);
        }

        #endregion

        private BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel;
        private CDS.ImageDisplay.Utils.SystemInfoPanel sysInfoPanel;
        private RegionOfInterest.SingleROIManager singleROIManager;
    }
}
