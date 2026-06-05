namespace CDS.ImageDisplay.Demo.DemoForms
{
    partial class FormLineSelectionDetailed
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
            labelPaintBackgroundMetrics = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            propertyGrid = new System.Windows.Forms.PropertyGrid();
            labelPaintForegroundMetrics = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            labelDraggingLine = new System.Windows.Forms.Label();
            labelCommittedLine = new System.Windows.Forms.Label();
            btnClearLine = new System.Windows.Forms.Button();
            btnSetLine = new System.Windows.Forms.Button();
            sysInfoPanel = new Utils.SystemInfoPanel();
            timerUpdateMetrics = new System.Windows.Forms.Timer(components);
            singleLineSelectionManager = new CDS.ImageDisplay.LineSelection.SingleLineSelectionManager(components);
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
            bitmapDisplayPanel.Size = new System.Drawing.Size(352, 412);
            bitmapDisplayPanel.TabIndex = 0;
            bitmapDisplayPanel.Zoom = 1F;
            // 
            // panel1
            // 
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel1.Controls.Add(labelPaintBackgroundMetrics);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(propertyGrid);
            panel1.Controls.Add(labelPaintForegroundMetrics);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(labelDraggingLine);
            panel1.Controls.Add(labelCommittedLine);
            panel1.Controls.Add(btnClearLine);
            panel1.Controls.Add(btnSetLine);
            panel1.Dock = System.Windows.Forms.DockStyle.Left;
            panel1.Location = new System.Drawing.Point(0, 38);
            panel1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(448, 412);
            panel1.TabIndex = 2;
            // 
            // labelPaintBackgroundMetrics
            // 
            labelPaintBackgroundMetrics.AutoSize = true;
            labelPaintBackgroundMetrics.Location = new System.Drawing.Point(119, 82);
            labelPaintBackgroundMetrics.Name = "labelPaintBackgroundMetrics";
            labelPaintBackgroundMetrics.Size = new System.Drawing.Size(25, 15);
            labelPaintBackgroundMetrics.TabIndex = 12;
            labelPaintBackgroundMetrics.Text = "n/a";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(11, 82);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(101, 15);
            label5.TabIndex = 11;
            label5.Text = "Paint background";
            // 
            // propertyGrid
            // 
            propertyGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            propertyGrid.Location = new System.Drawing.Point(11, 105);
            propertyGrid.Name = "propertyGrid";
            propertyGrid.Size = new System.Drawing.Size(424, 294);
            propertyGrid.TabIndex = 6;
            // 
            // labelPaintForegroundMetrics
            // 
            labelPaintForegroundMetrics.AutoSize = true;
            labelPaintForegroundMetrics.Location = new System.Drawing.Point(119, 67);
            labelPaintForegroundMetrics.Name = "labelPaintForegroundMetrics";
            labelPaintForegroundMetrics.Size = new System.Drawing.Size(25, 15);
            labelPaintForegroundMetrics.TabIndex = 10;
            labelPaintForegroundMetrics.Text = "n/a";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(11, 52);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(83, 15);
            label3.TabIndex = 9;
            label3.Text = "Dragging line";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(11, 37);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(95, 15);
            label2.TabIndex = 8;
            label2.Text = "Committed line";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(11, 67);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(97, 15);
            label1.TabIndex = 7;
            label1.Text = "Paint foreground";
            // 
            // labelDraggingLine
            // 
            labelDraggingLine.AutoSize = true;
            labelDraggingLine.Location = new System.Drawing.Point(119, 52);
            labelDraggingLine.Name = "labelDraggingLine";
            labelDraggingLine.Size = new System.Drawing.Size(25, 15);
            labelDraggingLine.TabIndex = 5;
            labelDraggingLine.Text = "n/a";
            // 
            // labelCommittedLine
            // 
            labelCommittedLine.AutoSize = true;
            labelCommittedLine.Location = new System.Drawing.Point(119, 37);
            labelCommittedLine.Name = "labelCommittedLine";
            labelCommittedLine.Size = new System.Drawing.Size(25, 15);
            labelCommittedLine.TabIndex = 4;
            labelCommittedLine.Text = "n/a";
            // 
            // btnClearLine
            // 
            btnClearLine.Location = new System.Drawing.Point(92, 11);
            btnClearLine.Name = "btnClearLine";
            btnClearLine.Size = new System.Drawing.Size(75, 23);
            btnClearLine.TabIndex = 3;
            btnClearLine.Text = "Clear line";
            btnClearLine.UseVisualStyleBackColor = true;
            btnClearLine.Click += btnClearLine_Click;
            // 
            // btnSetLine
            // 
            btnSetLine.Location = new System.Drawing.Point(11, 11);
            btnSetLine.Name = "btnSetLine";
            btnSetLine.Size = new System.Drawing.Size(75, 23);
            btnSetLine.TabIndex = 2;
            btnSetLine.Text = "Set line";
            btnSetLine.UseVisualStyleBackColor = true;
            btnSetLine.Click += btnSetLine_Click;
            // 
            // sysInfoPanel
            // 
            sysInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            sysInfoPanel.Location = new System.Drawing.Point(0, 0);
            sysInfoPanel.Name = "sysInfoPanel";
            sysInfoPanel.Size = new System.Drawing.Size(800, 38);
            sysInfoPanel.TabIndex = 7;
            // 
            // timerUpdateMetrics
            // 
            timerUpdateMetrics.Enabled = true;
            timerUpdateMetrics.Interval = 250;
            timerUpdateMetrics.Tick += timerUpdateMetrics_Tick;
            // 
            // singleLineSelectionManager
            // 
            singleLineSelectionManager.BitmapDisplayPanel = bitmapDisplayPanel;
            singleLineSelectionManager.CanCreateNew = true;
            singleLineSelectionManager.CanEditCommitted = true;
            singleLineSelectionManager.CommittedLineShape.Drawing.Fill.Color = System.Drawing.Color.Transparent;
            singleLineSelectionManager.CommittedLineShape.Drawing.Font.FontName = "Arial";
            singleLineSelectionManager.CommittedLineShape.Drawing.Font.FontSize = 12;
            singleLineSelectionManager.CommittedLineShape.Drawing.Lines.Color = System.Drawing.Color.FromArgb(128, 0, 128, 0);
            singleLineSelectionManager.CommittedLineShape.Drawing.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            singleLineSelectionManager.CommittedLineShape.Drawing.Lines.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            singleLineSelectionManager.CommittedLineShape.Drawing.Lines.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            singleLineSelectionManager.CommittedLineShape.Drawing.Lines.Width = 2F;
            singleLineSelectionManager.CommittedLineShape.Drawing.Visible = true;
            singleLineSelectionManager.CommittedLineShape.End = new System.Drawing.Point(0, 0);
            singleLineSelectionManager.CommittedLineShape.HandleDiameter = 6;
            singleLineSelectionManager.CommittedLineShape.HandlesVisible = true;
            singleLineSelectionManager.CommittedLineShape.PixelAlign = BitmapDisplay.DisplayPixelAlign.Centre;
            singleLineSelectionManager.CommittedLineShape.Start = new System.Drawing.Point(0, 0);
            singleLineSelectionManager.CommittedLineShape.Visible = true;
            singleLineSelectionManager.LiveDraggingLineShape.Drawing.Fill.Color = System.Drawing.Color.FromArgb(32, 255, 128, 0);
            singleLineSelectionManager.LiveDraggingLineShape.Drawing.Lines.Color = System.Drawing.Color.FromArgb(192, 255, 128, 0);
            singleLineSelectionManager.LiveDraggingLineShape.Drawing.Lines.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            singleLineSelectionManager.LiveDraggingLineShape.Drawing.Lines.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            singleLineSelectionManager.LiveDraggingLineShape.Drawing.Lines.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            singleLineSelectionManager.LiveDraggingLineShape.Drawing.Lines.Width = 2F;
            singleLineSelectionManager.LiveDraggingLineShape.Drawing.Visible = true;
            singleLineSelectionManager.LiveDraggingLineShape.End = new System.Drawing.Point(0, 0);
            singleLineSelectionManager.LiveDraggingLineShape.HandleDiameter = 6;
            singleLineSelectionManager.LiveDraggingLineShape.HandlesVisible = true;
            singleLineSelectionManager.LiveDraggingLineShape.PixelAlign = BitmapDisplay.DisplayPixelAlign.Centre;
            singleLineSelectionManager.LiveDraggingLineShape.Start = new System.Drawing.Point(0, 0);
            singleLineSelectionManager.LiveDraggingLineShape.Visible = true;
            singleLineSelectionManager.Visible = true;
            singleLineSelectionManager.CommittedLineChanged += singleLineSelectionManager_OnCommittedLineChanged;
            singleLineSelectionManager.DraggingLineChanged += singleLineSelectionManager_OnDraggingLineChanged;
            // 
            // FormLineSelectionDetailed
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(bitmapDisplayPanel);
            Controls.Add(panel1);
            Controls.Add(sysInfoPanel);
            Name = "FormLineSelectionDetailed";
            Text = "Line selection";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClearLine;
        private System.Windows.Forms.Button btnSetLine;
        private System.Windows.Forms.Label labelDraggingLine;
        private System.Windows.Forms.Label labelCommittedLine;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private CDS.ImageDisplay.Utils.SystemInfoPanel sysInfoPanel;
        private System.Windows.Forms.Label labelPaintBackgroundMetrics;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelPaintForegroundMetrics;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timerUpdateMetrics;
        private CDS.ImageDisplay.LineSelection.SingleLineSelectionManager singleLineSelectionManager;
    }
}
