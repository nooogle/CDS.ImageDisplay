
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
            components = new System.ComponentModel.Container();
            bitmapDisplayPanel = new WinForms.BitmapDisplay.BitmapDisplayPanel();
            panel1 = new System.Windows.Forms.Panel();
            labelPaintBackgroundMetrics = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            labelPaintForegroundMetrics = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            labelDraggingROI = new System.Windows.Forms.Label();
            labelCommittedROI = new System.Windows.Forms.Label();
            btnClearROI = new System.Windows.Forms.Button();
            btnSetROI = new System.Windows.Forms.Button();
            sysInfoPanel = new WinForms.SysInfoPanel();
            timerUpdateMetrics = new System.Windows.Forms.Timer(components);
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // bitmapDisplayPanel
            // 
            bitmapDisplayPanel.BackgroundImage = Properties.Resources.double_bubble;
            bitmapDisplayPanel.CanCreateNewROI = true;
            bitmapDisplayPanel.CanEditCommittedROI = true;
            bitmapDisplayPanel.CommittedROI = new System.Drawing.Rectangle(0, 0, 0, 0);
            bitmapDisplayPanel.DisplayMode = WinForms.BitmapDisplay.BitmapDisplayMode.Free;
            bitmapDisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapDisplayPanel.Location = new System.Drawing.Point(448, 38);
            bitmapDisplayPanel.MouseMode = WinForms.BitmapDisplay.MouseMode.ROISelection;
            bitmapDisplayPanel.Name = "bitmapDisplayPanel";
            bitmapDisplayPanel.ROIVisible = true;
            bitmapDisplayPanel.Size = new System.Drawing.Size(352, 412);
            bitmapDisplayPanel.TabIndex = 0;
            bitmapDisplayPanel.Zoom = 1F;
            bitmapDisplayPanel.OnCommittedROIChanged += bitmapDisplayPanel_CDSOnCommittedROIChanged;
            bitmapDisplayPanel.OnDraggingROIChanged += bitmapDisplayPanel_CDSOnDraggingROIChanged;
            // 
            // panel1
            // 
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel1.Controls.Add(labelPaintBackgroundMetrics);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(labelPaintForegroundMetrics);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(propertyGrid1);
            panel1.Controls.Add(labelDraggingROI);
            panel1.Controls.Add(labelCommittedROI);
            panel1.Controls.Add(btnClearROI);
            panel1.Controls.Add(btnSetROI);
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
            label3.Size = new System.Drawing.Size(78, 15);
            label3.TabIndex = 9;
            label3.Text = "Dragging ROI";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(11, 37);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(90, 15);
            label2.TabIndex = 8;
            label2.Text = "Committed ROI";
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
            // propertyGrid1
            // 
            propertyGrid1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            propertyGrid1.Location = new System.Drawing.Point(10, 132);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.Size = new System.Drawing.Size(428, 267);
            propertyGrid1.TabIndex = 6;
            // 
            // labelDraggingROI
            // 
            labelDraggingROI.AutoSize = true;
            labelDraggingROI.Location = new System.Drawing.Point(119, 52);
            labelDraggingROI.Name = "labelDraggingROI";
            labelDraggingROI.Size = new System.Drawing.Size(25, 15);
            labelDraggingROI.TabIndex = 5;
            labelDraggingROI.Text = "n/a";
            // 
            // labelCommittedROI
            // 
            labelCommittedROI.AutoSize = true;
            labelCommittedROI.Location = new System.Drawing.Point(119, 37);
            labelCommittedROI.Name = "labelCommittedROI";
            labelCommittedROI.Size = new System.Drawing.Size(25, 15);
            labelCommittedROI.TabIndex = 4;
            labelCommittedROI.Text = "n/a";
            // 
            // btnClearROI
            // 
            btnClearROI.Location = new System.Drawing.Point(92, 11);
            btnClearROI.Name = "btnClearROI";
            btnClearROI.Size = new System.Drawing.Size(75, 23);
            btnClearROI.TabIndex = 3;
            btnClearROI.Text = "Clear ROI";
            btnClearROI.UseVisualStyleBackColor = true;
            btnClearROI.Click += btnClearROI_Click;
            // 
            // btnSetROI
            // 
            btnSetROI.Location = new System.Drawing.Point(11, 11);
            btnSetROI.Name = "btnSetROI";
            btnSetROI.Size = new System.Drawing.Size(75, 23);
            btnSetROI.TabIndex = 2;
            btnSetROI.Text = "Set ROI";
            btnSetROI.UseVisualStyleBackColor = true;
            btnSetROI.Click += btnSetROI_Click;
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
            // FormROISelection
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(bitmapDisplayPanel);
            Controls.Add(panel1);
            Controls.Add(sysInfoPanel);
            Name = "FormROISelection";
            Text = "ROI selection";
            Load += FormROISelection_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClearROI;
        private System.Windows.Forms.Button btnSetROI;
        private System.Windows.Forms.Label labelDraggingROI;
        private System.Windows.Forms.Label labelCommittedROI;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private CDS.Imaging.WinForms.SysInfoPanel sysInfoPanel;
        private System.Windows.Forms.Label labelPaintBackgroundMetrics;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelPaintForegroundMetrics;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timerUpdateMetrics;
    }
}