
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
            panel1 = new System.Windows.Forms.Panel();
            propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            labelDraggingROI = new System.Windows.Forms.Label();
            labelCommittedROI = new System.Windows.Forms.Label();
            btnClearROI = new System.Windows.Forms.Button();
            btnSetROI = new System.Windows.Forms.Button();
            sysInfoPanel = new WinForms.SysInfoPanel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // bitmapDisplayPanel
            // 
            bitmapDisplayPanel.BackgroundImage = Properties.Resources.double_bubble;
            bitmapDisplayPanel.CanCreateNewROI = false;
            bitmapDisplayPanel.CanEditCommittedROI = false;
            bitmapDisplayPanel.CommittedROI = new System.Drawing.Rectangle(0, 0, 0, 0);
            bitmapDisplayPanel.DisplayMode = WinForms.BitmapDisplay.BitmapDisplayMode.Free;
            bitmapDisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapDisplayPanel.Location = new System.Drawing.Point(310, 38);
            bitmapDisplayPanel.MouseMode = WinForms.BitmapDisplay.MouseMode.None;
            bitmapDisplayPanel.Name = "bitmapDisplayPanel";
            bitmapDisplayPanel.ROIVisible = true;
            bitmapDisplayPanel.Size = new System.Drawing.Size(490, 412);
            bitmapDisplayPanel.TabIndex = 0;
            bitmapDisplayPanel.Zoom = 1F;
            bitmapDisplayPanel.OnCommittedROIChanged += bitmapDisplayPanel_CDSOnCommittedROIChanged;
            bitmapDisplayPanel.OnDraggingROIChanged += bitmapDisplayPanel_CDSOnDraggingROIChanged;
            // 
            // panel1
            // 
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel1.Controls.Add(propertyGrid1);
            panel1.Controls.Add(labelDraggingROI);
            panel1.Controls.Add(labelCommittedROI);
            panel1.Controls.Add(btnClearROI);
            panel1.Controls.Add(btnSetROI);
            panel1.Dock = System.Windows.Forms.DockStyle.Left;
            panel1.Location = new System.Drawing.Point(0, 38);
            panel1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(310, 412);
            panel1.TabIndex = 2;
            // 
            // propertyGrid1
            // 
            propertyGrid1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            propertyGrid1.Location = new System.Drawing.Point(10, 74);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.Size = new System.Drawing.Size(290, 325);
            propertyGrid1.TabIndex = 6;
            // 
            // labelDraggingROI
            // 
            labelDraggingROI.AutoSize = true;
            labelDraggingROI.Location = new System.Drawing.Point(11, 52);
            labelDraggingROI.Name = "labelDraggingROI";
            labelDraggingROI.Size = new System.Drawing.Size(81, 15);
            labelDraggingROI.TabIndex = 5;
            labelDraggingROI.Text = "Dragging ROI:";
            // 
            // labelCommittedROI
            // 
            labelCommittedROI.AutoSize = true;
            labelCommittedROI.Location = new System.Drawing.Point(11, 37);
            labelCommittedROI.Name = "labelCommittedROI";
            labelCommittedROI.Size = new System.Drawing.Size(93, 15);
            labelCommittedROI.TabIndex = 4;
            labelCommittedROI.Text = "Committed ROI:";
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
    }
}