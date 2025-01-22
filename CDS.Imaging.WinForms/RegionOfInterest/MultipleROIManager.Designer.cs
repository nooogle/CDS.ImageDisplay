namespace CDS.Imaging.RegionOfInterest
{
    partial class MultipleROIManager
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            roiSelectionOnBitmapDisplay = new SingleROIManager(components);
            timerAutoDeselectActiveROI = new System.Windows.Forms.Timer(components);
            // 
            // roiSelectionOnBitmapDisplay
            // 
            roiSelectionOnBitmapDisplay.BitmapDisplayPanel = null;
            roiSelectionOnBitmapDisplay.CanCreateNew = false;
            roiSelectionOnBitmapDisplay.CanEditCommitted = false;
            roiSelectionOnBitmapDisplay.CommittedROI = new System.Drawing.Rectangle(0, 0, 0, 0);
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.GrappleDiameter = 6;
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.GrapplesVisible = true;
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.Locked = false;
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.MaximumSize = new System.Drawing.Size(1000000, 1000000);
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.MinimumSize = new System.Drawing.Size(1, 1);
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.Name = "";
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.PixelAlign = BitmapDisplay.DisplayPixelAlign.TopLeft;
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.Rendering.Fill.Color = System.Drawing.Color.Transparent;
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.Rendering.Font.FontName = "Arial";
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.Rendering.Font.FontSize = 12;
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.Rendering.Lines.Color = System.Drawing.Color.FromArgb(128, 0, 128, 0);
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.Rendering.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.Rendering.Lines.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.Rendering.Lines.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.Rendering.Lines.Width = 2F;
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.Rendering.Visible = true;
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.ROI = new System.Drawing.Rectangle(0, 0, 0, 0);
            roiSelectionOnBitmapDisplay.CommittedROIRenderer.Visible = true;
            roiSelectionOnBitmapDisplay.DragBorder = 10;
            roiSelectionOnBitmapDisplay.DrawCommittedROIWhenFullSize = true;
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.GrappleDiameter = 6;
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.GrapplesVisible = true;
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.Locked = false;
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.MaximumSize = new System.Drawing.Size(1000000, 1000000);
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.MinimumSize = new System.Drawing.Size(1, 1);
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.Name = "";
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.PixelAlign = BitmapDisplay.DisplayPixelAlign.TopLeft;
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.Rendering.Fill.Color = System.Drawing.Color.Transparent;
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.Rendering.Font.FontName = "Arial";
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.Rendering.Font.FontSize = 12;
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.Rendering.Lines.Color = System.Drawing.Color.FromArgb(128, 255, 165, 0);
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.Rendering.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.Rendering.Lines.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.Rendering.Lines.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.Rendering.Lines.Width = 2F;
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.Rendering.Visible = true;
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.ROI = new System.Drawing.Rectangle(0, 0, 0, 0);
            roiSelectionOnBitmapDisplay.LiveDraggingROIRenderer.Visible = true;
            roiSelectionOnBitmapDisplay.Visible = false;
            roiSelectionOnBitmapDisplay.OnCommittedROIChanged += roiSelectionOnBitmapDisplay_OnCommittedROIChanged;
            roiSelectionOnBitmapDisplay.OnDraggingROIChanged += roiSelectionOnBitmapDisplay_OnDraggingROIChanged;
            // 
            // timerAutoDeselectActiveROI
            // 
            timerAutoDeselectActiveROI.Interval = 1000;
            timerAutoDeselectActiveROI.Tick += timerAutoDeselectActiveROI_Tick;
        }

        #endregion

        private SingleROIManager roiSelectionOnBitmapDisplay;
        private System.Windows.Forms.Timer timerAutoDeselectActiveROI;
    }
}
