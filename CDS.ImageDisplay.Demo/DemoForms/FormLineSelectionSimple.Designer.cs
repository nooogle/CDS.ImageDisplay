namespace CDS.ImageDisplay.Demo.DemoForms
{
    partial class FormLineSelectionSimple
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
            singleLineSelectionManager = new CDS.ImageDisplay.LineSelection.SingleLineSelectionManager(components);
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
            bitmapDisplayPanel.PaintOver += bitmapDisplayPanel_OnPaintOver;
            // 
            // sysInfoPanel
            // 
            sysInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            sysInfoPanel.Location = new System.Drawing.Point(0, 0);
            sysInfoPanel.Name = "sysInfoPanel";
            sysInfoPanel.Size = new System.Drawing.Size(800, 38);
            sysInfoPanel.TabIndex = 7;
            // 
            // singleLineSelectionManager
            // 
            singleLineSelectionManager.BitmapDisplayPanel = bitmapDisplayPanel;
            singleLineSelectionManager.CommittedLineDrawingSpec.Fill.Color = System.Drawing.Color.Transparent;
            singleLineSelectionManager.CommittedLineDrawingSpec.Font.FontName = "Arial";
            singleLineSelectionManager.CommittedLineDrawingSpec.Font.FontSize = 12;
            singleLineSelectionManager.CommittedLineDrawingSpec.Font.FontStyle = System.Drawing.FontStyle.Regular;
            singleLineSelectionManager.CommittedLineDrawingSpec.Lines.Color = System.Drawing.Color.FromArgb(128, 0, 128, 0);
            singleLineSelectionManager.CommittedLineDrawingSpec.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            singleLineSelectionManager.CommittedLineDrawingSpec.Lines.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            singleLineSelectionManager.CommittedLineDrawingSpec.Lines.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            singleLineSelectionManager.CommittedLineDrawingSpec.Lines.Width = 2F;
            singleLineSelectionManager.CommittedLineDrawingSpec.MappingMode = Overlays.MappingMode.ImageToDisplay;
            singleLineSelectionManager.CommittedLineDrawingSpec.Visible = true;
            singleLineSelectionManager.CommittedLineShape.Drawing = singleLineSelectionManager.CommittedLineDrawingSpec;
            singleLineSelectionManager.CommittedLineShape.End = new System.Drawing.Point(0, 0);
            singleLineSelectionManager.CommittedLineShape.HandleDiameter = 6;
            singleLineSelectionManager.CommittedLineShape.HandlesVisible = true;
            singleLineSelectionManager.CommittedLineShape.PixelAlign = BitmapDisplay.DisplayPixelAlign.Centre;
            singleLineSelectionManager.CommittedLineShape.Start = new System.Drawing.Point(0, 0);
            singleLineSelectionManager.CommittedLineShape.Visible = true;
            singleLineSelectionManager.LiveDraggingLineDrawingSpec.Fill.Color = System.Drawing.Color.FromArgb(32, 255, 128, 0);
            singleLineSelectionManager.LiveDraggingLineDrawingSpec.Font.FontName = "Arial";
            singleLineSelectionManager.LiveDraggingLineDrawingSpec.Font.FontSize = 12;
            singleLineSelectionManager.LiveDraggingLineDrawingSpec.Font.FontStyle = System.Drawing.FontStyle.Regular;
            singleLineSelectionManager.LiveDraggingLineDrawingSpec.Lines.Color = System.Drawing.Color.FromArgb(192, 255, 128, 0);
            singleLineSelectionManager.LiveDraggingLineDrawingSpec.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            singleLineSelectionManager.LiveDraggingLineDrawingSpec.Lines.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            singleLineSelectionManager.LiveDraggingLineDrawingSpec.Lines.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            singleLineSelectionManager.LiveDraggingLineDrawingSpec.Lines.Width = 2F;
            singleLineSelectionManager.LiveDraggingLineDrawingSpec.MappingMode = Overlays.MappingMode.ImageToDisplay;
            singleLineSelectionManager.LiveDraggingLineDrawingSpec.Visible = true;
            singleLineSelectionManager.LiveDraggingLineShape.Drawing = singleLineSelectionManager.LiveDraggingLineDrawingSpec;
            singleLineSelectionManager.LiveDraggingLineShape.End = new System.Drawing.Point(0, 0);
            singleLineSelectionManager.LiveDraggingLineShape.HandleDiameter = 6;
            singleLineSelectionManager.LiveDraggingLineShape.HandlesVisible = true;
            singleLineSelectionManager.LiveDraggingLineShape.PixelAlign = BitmapDisplay.DisplayPixelAlign.Centre;
            singleLineSelectionManager.LiveDraggingLineShape.Start = new System.Drawing.Point(0, 0);
            singleLineSelectionManager.LiveDraggingLineShape.Visible = true;
            singleLineSelectionManager.CommittedLineChanged += singleLineSelectionManager_OnCommittedLineChanged;
            singleLineSelectionManager.DraggingLineChanged += singleLineSelectionManager_OnDraggingLineChanged;
            // 
            // FormLineSelectionSimple
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(bitmapDisplayPanel);
            Controls.Add(sysInfoPanel);
            Name = "FormLineSelectionSimple";
            Text = "Line selection";
            ResumeLayout(false);
        }

        #endregion

        private BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel;
        private CDS.ImageDisplay.Utils.SystemInfoPanel sysInfoPanel;
        private CDS.ImageDisplay.LineSelection.SingleLineSelectionManager singleLineSelectionManager;
    }
}
