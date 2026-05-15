namespace CDS.ImageDisplay.RegionOfInterest
{
    partial class MultipleROIManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            roiSelectionOnBitmapDisplay = new SingleROIManager(components);
            //
            // roiSelectionOnBitmapDisplay
            //
            roiSelectionOnBitmapDisplay.BitmapDisplayPanel = null;
            roiSelectionOnBitmapDisplay.CanCreateNew = false;
            roiSelectionOnBitmapDisplay.CanEditCommitted = false;
            roiSelectionOnBitmapDisplay.CommittedROI = new System.Drawing.Rectangle(0, 0, 0, 0);
            roiSelectionOnBitmapDisplay.CommittedROIShape.GrappleDiameter = 6;
            roiSelectionOnBitmapDisplay.CommittedROIShape.GrapplesVisible = true;
            roiSelectionOnBitmapDisplay.CommittedROIShape.Locked = false;
            roiSelectionOnBitmapDisplay.CommittedROIShape.MaximumSize = new System.Drawing.Size(1000000, 1000000);
            roiSelectionOnBitmapDisplay.CommittedROIShape.MinimumSize = new System.Drawing.Size(1, 1);
            roiSelectionOnBitmapDisplay.CommittedROIShape.Name = "";
            roiSelectionOnBitmapDisplay.CommittedROIShape.PixelAlign = BitmapDisplay.DisplayPixelAlign.TopLeft;
            roiSelectionOnBitmapDisplay.CommittedROIShape.Drawing.Fill.Color = System.Drawing.Color.Transparent;
            roiSelectionOnBitmapDisplay.CommittedROIShape.Drawing.Font.FontName = "Arial";
            roiSelectionOnBitmapDisplay.CommittedROIShape.Drawing.Font.FontSize = 12;
            roiSelectionOnBitmapDisplay.CommittedROIShape.Drawing.Lines.Color = System.Drawing.Color.FromArgb(128, 0, 128, 0);
            roiSelectionOnBitmapDisplay.CommittedROIShape.Drawing.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            roiSelectionOnBitmapDisplay.CommittedROIShape.Drawing.Lines.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            roiSelectionOnBitmapDisplay.CommittedROIShape.Drawing.Lines.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            roiSelectionOnBitmapDisplay.CommittedROIShape.Drawing.Lines.Width = 2F;
            roiSelectionOnBitmapDisplay.CommittedROIShape.Drawing.Visible = true;
            roiSelectionOnBitmapDisplay.CommittedROIShape.ROI = new System.Drawing.Rectangle(0, 0, 0, 0);
            roiSelectionOnBitmapDisplay.CommittedROIShape.Visible = true;
            roiSelectionOnBitmapDisplay.DragBorder = 10;
            roiSelectionOnBitmapDisplay.DrawCommittedROIWhenFullSize = true;
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.GrappleDiameter = 6;
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.GrapplesVisible = true;
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.Locked = false;
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.MaximumSize = new System.Drawing.Size(1000000, 1000000);
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.MinimumSize = new System.Drawing.Size(1, 1);
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.Name = "";
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.PixelAlign = BitmapDisplay.DisplayPixelAlign.TopLeft;
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.Drawing.Fill.Color = System.Drawing.Color.Transparent;
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.Drawing.Font.FontName = "Arial";
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.Drawing.Font.FontSize = 12;
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.Drawing.Lines.Color = System.Drawing.Color.FromArgb(128, 255, 165, 0);
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.Drawing.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.Drawing.Lines.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.Drawing.Lines.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.Drawing.Lines.Width = 2F;
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.Drawing.Visible = true;
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.ROI = new System.Drawing.Rectangle(0, 0, 0, 0);
            roiSelectionOnBitmapDisplay.LiveDraggingROIShape.Visible = true;
            roiSelectionOnBitmapDisplay.Visible = false;
            roiSelectionOnBitmapDisplay.OnCommittedROIChanged += roiSelectionOnBitmapDisplay_OnCommittedROIChanged;
            roiSelectionOnBitmapDisplay.OnDraggingROIChanged += roiSelectionOnBitmapDisplay_OnDraggingROIChanged;
        }

        #endregion

        private SingleROIManager roiSelectionOnBitmapDisplay;
    }
}
