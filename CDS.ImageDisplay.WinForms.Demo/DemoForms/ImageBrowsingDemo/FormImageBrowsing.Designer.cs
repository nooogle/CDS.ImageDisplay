namespace CDS.ImageDisplay.WinForms.Demo.DemoForms.ImageBrowsingDemo
{
    partial class FormImageBrowsing
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
            splitContainer = new System.Windows.Forms.SplitContainer();
            imageListPanel = new CDS.ImageDisplay.WinForms.ImageBrowsing.ImageListPanel();
            bitmapDisplayPanel = new CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel();
            panelToolbar = new System.Windows.Forms.Panel();
            btnNext = new System.Windows.Forms.Button();
            btnPrevious = new System.Windows.Forms.Button();
            nudThumbnailSize = new System.Windows.Forms.NumericUpDown();
            lblThumbnailSize = new System.Windows.Forms.Label();
            btnBrowse = new System.Windows.Forms.Button();
            txtFolder = new System.Windows.Forms.TextBox();
            lblFolder = new System.Windows.Forms.Label();
            labelStatus = new System.Windows.Forms.Label();
            labelTime = new System.Windows.Forms.Label();
            timerTime = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            panelToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudThumbnailSize).BeginInit();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer.Location = new System.Drawing.Point(0, 35);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(imageListPanel);
            splitContainer.Panel1MinSize = 120;
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(bitmapDisplayPanel);
            splitContainer.Size = new System.Drawing.Size(984, 550);
            splitContainer.SplitterDistance = 368;
            splitContainer.TabIndex = 0;
            // 
            // imageListPanel
            // 
            imageListPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            imageListPanel.Location = new System.Drawing.Point(0, 0);
            imageListPanel.Name = "imageListPanel";
            imageListPanel.Size = new System.Drawing.Size(368, 550);
            imageListPanel.TabIndex = 0;
            imageListPanel.ThumbnailHeight = 128;
            imageListPanel.SelectionChanged += imageListPanel_SelectionChanged;
            // 
            // bitmapDisplayPanel
            // 
            bitmapDisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapDisplayPanel.Location = new System.Drawing.Point(0, 0);
            bitmapDisplayPanel.Name = "bitmapDisplayPanel";
            bitmapDisplayPanel.Size = new System.Drawing.Size(612, 550);
            bitmapDisplayPanel.TabIndex = 0;
            // 
            // panelToolbar
            // 
            panelToolbar.Controls.Add(labelTime);
            panelToolbar.Controls.Add(btnNext);
            panelToolbar.Controls.Add(btnPrevious);
            panelToolbar.Controls.Add(nudThumbnailSize);
            panelToolbar.Controls.Add(lblThumbnailSize);
            panelToolbar.Controls.Add(btnBrowse);
            panelToolbar.Controls.Add(txtFolder);
            panelToolbar.Controls.Add(lblFolder);
            panelToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            panelToolbar.Location = new System.Drawing.Point(0, 0);
            panelToolbar.Name = "panelToolbar";
            panelToolbar.Size = new System.Drawing.Size(984, 35);
            panelToolbar.TabIndex = 1;
            // 
            // btnNext
            // 
            btnNext.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnNext.Location = new System.Drawing.Point(949, 6);
            btnNext.Name = "btnNext";
            btnNext.Size = new System.Drawing.Size(35, 23);
            btnNext.TabIndex = 6;
            btnNext.Text = "▶";
            btnNext.Click += btnNext_Click;
            // 
            // btnPrevious
            // 
            btnPrevious.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnPrevious.Location = new System.Drawing.Point(908, 6);
            btnPrevious.Name = "btnPrevious";
            btnPrevious.Size = new System.Drawing.Size(35, 23);
            btnPrevious.TabIndex = 5;
            btnPrevious.Text = "◀";
            btnPrevious.Click += btnPrevious_Click;
            // 
            // nudThumbnailSize
            // 
            nudThumbnailSize.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            nudThumbnailSize.Increment = new decimal(new int[] { 16, 0, 0, 0 });
            nudThumbnailSize.Location = new System.Drawing.Point(843, 6);
            nudThumbnailSize.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            nudThumbnailSize.Minimum = new decimal(new int[] { 16, 0, 0, 0 });
            nudThumbnailSize.Name = "nudThumbnailSize";
            nudThumbnailSize.Size = new System.Drawing.Size(59, 23);
            nudThumbnailSize.TabIndex = 4;
            nudThumbnailSize.Value = new decimal(new int[] { 128, 0, 0, 0 });
            nudThumbnailSize.ValueChanged += nudThumbnailSize_ValueChanged;
            // 
            // lblThumbnailSize
            // 
            lblThumbnailSize.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblThumbnailSize.AutoSize = true;
            lblThumbnailSize.Location = new System.Drawing.Point(806, 10);
            lblThumbnailSize.Name = "lblThumbnailSize";
            lblThumbnailSize.Size = new System.Drawing.Size(30, 15);
            lblThumbnailSize.TabIndex = 3;
            lblThumbnailSize.Text = "Size:";
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnBrowse.Location = new System.Drawing.Point(725, 6);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new System.Drawing.Size(75, 23);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "Browse...";
            btnBrowse.Click += btnBrowse_Click;
            // 
            // txtFolder
            // 
            txtFolder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtFolder.Location = new System.Drawing.Point(162, 7);
            txtFolder.Name = "txtFolder";
            txtFolder.ReadOnly = true;
            txtFolder.Size = new System.Drawing.Size(557, 23);
            txtFolder.TabIndex = 1;
            // 
            // lblFolder
            // 
            lblFolder.AutoSize = true;
            lblFolder.Location = new System.Drawing.Point(113, 10);
            lblFolder.Name = "lblFolder";
            lblFolder.Size = new System.Drawing.Size(43, 15);
            lblFolder.TabIndex = 0;
            lblFolder.Text = "Folder:";
            // 
            // labelStatus
            // 
            labelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            labelStatus.Location = new System.Drawing.Point(0, 585);
            labelStatus.Name = "labelStatus";
            labelStatus.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            labelStatus.Size = new System.Drawing.Size(984, 24);
            labelStatus.TabIndex = 2;
            labelStatus.Text = "Select an image from the list";
            // 
            // labelTime
            // 
            labelTime.AutoSize = true;
            labelTime.Location = new System.Drawing.Point(12, 10);
            labelTime.Name = "labelTime";
            labelTime.Size = new System.Drawing.Size(78, 15);
            labelTime.TabIndex = 7;
            labelTime.Text = "HH:mm:ss.fff";
            // 
            // timerTime
            // 
            timerTime.Enabled = true;
            timerTime.Tick += timerTime_Tick;
            // 
            // FormImageBrowsing
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(984, 609);
            Controls.Add(splitContainer);
            Controls.Add(panelToolbar);
            Controls.Add(labelStatus);
            Name = "FormImageBrowsing";
            Text = "Image browsing";
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            panelToolbar.ResumeLayout(false);
            panelToolbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudThumbnailSize).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private CDS.ImageDisplay.WinForms.ImageBrowsing.ImageListPanel imageListPanel;
        private CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel;
        private System.Windows.Forms.Panel panelToolbar;
        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.NumericUpDown nudThumbnailSize;
        private System.Windows.Forms.Label lblThumbnailSize;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Timer timerTime;
    }
}
