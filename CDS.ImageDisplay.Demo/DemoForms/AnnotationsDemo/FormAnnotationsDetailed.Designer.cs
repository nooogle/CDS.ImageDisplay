namespace CDS.ImageDisplay.Demo.DemoForms.AnnotationsDemo
{
    partial class FormAnnotationsDetailed
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
            leftPanel = new System.Windows.Forms.Panel();
            checkCanCreateAnnotations = new System.Windows.Forms.CheckBox();
            checkCanEditAnnotations = new System.Windows.Forms.CheckBox();
            listView = new System.Windows.Forms.ListView();
            colType = new System.Windows.Forms.ColumnHeader();
            colTitle = new System.Windows.Forms.ColumnHeader();
            labelTitle = new System.Windows.Forms.Label();
            txtTitle = new System.Windows.Forms.TextBox();
            labelNotes = new System.Windows.Forms.Label();
            txtNotes = new System.Windows.Forms.TextBox();
            btnClearAll = new System.Windows.Forms.Button();
            btnSaveJson = new System.Windows.Forms.Button();
            btnLoadJson = new System.Windows.Forms.Button();
            statusStrip = new System.Windows.Forms.StatusStrip();
            statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            annotationManager = new CDS.ImageDisplay.Annotations.AnnotationManager(components);
            btnClearAnnotations = new System.Windows.Forms.Button();
            leftPanel.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // bitmapDisplayPanel
            // 
            bitmapDisplayPanel.BackgroundImage = Properties.Resources.double_bubble;
            bitmapDisplayPanel.DisplayMode = BitmapDisplay.BitmapDisplayMode.Free;
            bitmapDisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapDisplayPanel.Location = new System.Drawing.Point(280, 38);
            bitmapDisplayPanel.Name = "bitmapDisplayPanel";
            bitmapDisplayPanel.Size = new System.Drawing.Size(744, 540);
            bitmapDisplayPanel.TabIndex = 3;
            // 
            // sysInfoPanel
            // 
            sysInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            sysInfoPanel.Location = new System.Drawing.Point(0, 0);
            sysInfoPanel.Name = "sysInfoPanel";
            sysInfoPanel.Size = new System.Drawing.Size(1024, 38);
            sysInfoPanel.TabIndex = 0;
            // 
            // leftPanel
            // 
            leftPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            leftPanel.Controls.Add(btnClearAnnotations);
            leftPanel.Controls.Add(checkCanCreateAnnotations);
            leftPanel.Controls.Add(checkCanEditAnnotations);
            leftPanel.Controls.Add(listView);
            leftPanel.Controls.Add(labelTitle);
            leftPanel.Controls.Add(txtTitle);
            leftPanel.Controls.Add(labelNotes);
            leftPanel.Controls.Add(txtNotes);
            leftPanel.Controls.Add(btnClearAll);
            leftPanel.Controls.Add(btnSaveJson);
            leftPanel.Controls.Add(btnLoadJson);
            leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            leftPanel.Location = new System.Drawing.Point(0, 38);
            leftPanel.Name = "leftPanel";
            leftPanel.Size = new System.Drawing.Size(280, 540);
            leftPanel.TabIndex = 2;
            // 
            // checkCanCreateAnnotations
            // 
            checkCanCreateAnnotations.AutoSize = true;
            checkCanCreateAnnotations.Checked = true;
            checkCanCreateAnnotations.CheckState = System.Windows.Forms.CheckState.Checked;
            checkCanCreateAnnotations.Location = new System.Drawing.Point(87, 5);
            checkCanCreateAnnotations.Name = "checkCanCreateAnnotations";
            checkCanCreateAnnotations.Size = new System.Drawing.Size(82, 19);
            checkCanCreateAnnotations.TabIndex = 9;
            checkCanCreateAnnotations.Text = "Can create";
            checkCanCreateAnnotations.UseVisualStyleBackColor = true;
            checkCanCreateAnnotations.CheckedChanged += checkCanCreateAnnotations_CheckedChanged;
            // 
            // checkCanEditAnnotations
            // 
            checkCanEditAnnotations.AutoSize = true;
            checkCanEditAnnotations.Checked = true;
            checkCanEditAnnotations.CheckState = System.Windows.Forms.CheckState.Checked;
            checkCanEditAnnotations.Location = new System.Drawing.Point(11, 5);
            checkCanEditAnnotations.Name = "checkCanEditAnnotations";
            checkCanEditAnnotations.Size = new System.Drawing.Size(70, 19);
            checkCanEditAnnotations.TabIndex = 8;
            checkCanEditAnnotations.Text = "Can edit";
            checkCanEditAnnotations.UseVisualStyleBackColor = true;
            checkCanEditAnnotations.CheckedChanged += checkCanEditAnnotations_CheckedChanged;
            // 
            // listView
            // 
            listView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colType, colTitle });
            listView.FullRowSelect = true;
            listView.Location = new System.Drawing.Point(4, 30);
            listView.MultiSelect = false;
            listView.Name = "listView";
            listView.Size = new System.Drawing.Size(270, 343);
            listView.TabIndex = 0;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = System.Windows.Forms.View.Details;
            // 
            // colType
            // 
            colType.Text = "Type";
            colType.Width = 90;
            // 
            // colTitle
            // 
            colTitle.Text = "Title";
            colTitle.Width = 174;
            // 
            // labelTitle
            // 
            labelTitle.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            labelTitle.AutoSize = true;
            labelTitle.Location = new System.Drawing.Point(4, 377);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new System.Drawing.Size(33, 15);
            labelTitle.TabIndex = 1;
            labelTitle.Text = "Title:";
            // 
            // txtTitle
            // 
            txtTitle.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtTitle.Enabled = false;
            txtTitle.Location = new System.Drawing.Point(4, 394);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new System.Drawing.Size(270, 23);
            txtTitle.TabIndex = 2;
            txtTitle.TextChanged += txtTitle_TextChanged;
            // 
            // labelNotes
            // 
            labelNotes.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            labelNotes.AutoSize = true;
            labelNotes.Location = new System.Drawing.Point(4, 423);
            labelNotes.Name = "labelNotes";
            labelNotes.Size = new System.Drawing.Size(41, 15);
            labelNotes.TabIndex = 3;
            labelNotes.Text = "Notes:";
            // 
            // txtNotes
            // 
            txtNotes.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtNotes.Enabled = false;
            txtNotes.Location = new System.Drawing.Point(4, 440);
            txtNotes.Multiline = true;
            txtNotes.Name = "txtNotes";
            txtNotes.Size = new System.Drawing.Size(270, 64);
            txtNotes.TabIndex = 4;
            txtNotes.TextChanged += txtNotes_TextChanged;
            // 
            // btnClearAll
            // 
            btnClearAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnClearAll.Location = new System.Drawing.Point(4, 510);
            btnClearAll.Name = "btnClearAll";
            btnClearAll.Size = new System.Drawing.Size(82, 26);
            btnClearAll.TabIndex = 5;
            btnClearAll.Text = "Clear all";
            btnClearAll.UseVisualStyleBackColor = true;
            btnClearAll.Click += btnClearAll_Click;
            // 
            // btnSaveJson
            // 
            btnSaveJson.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnSaveJson.Location = new System.Drawing.Point(90, 510);
            btnSaveJson.Name = "btnSaveJson";
            btnSaveJson.Size = new System.Drawing.Size(82, 26);
            btnSaveJson.TabIndex = 6;
            btnSaveJson.Text = "Save JSON";
            btnSaveJson.UseVisualStyleBackColor = true;
            btnSaveJson.Click += btnSaveJson_Click;
            // 
            // btnLoadJson
            // 
            btnLoadJson.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnLoadJson.Location = new System.Drawing.Point(176, 510);
            btnLoadJson.Name = "btnLoadJson";
            btnLoadJson.Size = new System.Drawing.Size(82, 26);
            btnLoadJson.TabIndex = 7;
            btnLoadJson.Text = "Load JSON";
            btnLoadJson.UseVisualStyleBackColor = true;
            btnLoadJson.Click += btnLoadJson_Click;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { statusLabel });
            statusStrip.Location = new System.Drawing.Point(0, 578);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new System.Drawing.Size(1024, 22);
            statusStrip.TabIndex = 1;
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new System.Drawing.Size(39, 17);
            statusLabel.Text = "Ready";
            // 
            // annotationManager
            // 
            annotationManager.BitmapDisplayPanel = bitmapDisplayPanel;
            annotationManager.AnnotationCreated += annotationManager_AnnotationCreated;
            annotationManager.AnnotationModified += annotationManager_AnnotationModified;
            annotationManager.AnnotationDeleted += annotationManager_AnnotationDeleted;
            annotationManager.AnnotationSelected += annotationManager_AnnotationSelected;
            annotationManager.AnnotationDeselected += annotationManager_AnnotationDeselected;
            // 
            // btnClearAnnotations
            // 
            btnClearAnnotations.Location = new System.Drawing.Point(175, 2);
            btnClearAnnotations.Name = "btnClearAnnotations";
            btnClearAnnotations.Size = new System.Drawing.Size(75, 23);
            btnClearAnnotations.TabIndex = 10;
            btnClearAnnotations.Text = "Clear";
            btnClearAnnotations.UseVisualStyleBackColor = true;
            btnClearAnnotations.Click += btnClearAnnotations_Click;
            // 
            // FormAnnotationsDetailed
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1024, 600);
            Controls.Add(bitmapDisplayPanel);
            Controls.Add(leftPanel);
            Controls.Add(sysInfoPanel);
            Controls.Add(statusStrip);
            Name = "FormAnnotationsDetailed";
            Text = "Annotations: detailed";
            leftPanel.ResumeLayout(false);
            leftPanel.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel;
        private CDS.ImageDisplay.Utils.SystemInfoPanel sysInfoPanel;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colTitle;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label labelNotes;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.Button btnSaveJson;
        private System.Windows.Forms.Button btnLoadJson;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private CDS.ImageDisplay.Annotations.AnnotationManager annotationManager;
        private System.Windows.Forms.CheckBox checkCanEditAnnotations;
        private System.Windows.Forms.CheckBox checkCanCreateAnnotations;
        private System.Windows.Forms.Button btnClearAnnotations;
    }
}
