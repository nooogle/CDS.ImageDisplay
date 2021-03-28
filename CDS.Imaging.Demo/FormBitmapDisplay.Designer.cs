
namespace CDS.Imaging.Demo
{
    partial class FormBitmapDisplay
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bitmapDisplay = new CDS.Imaging.WinForms.BitmapDisplay();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.menuImage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuImageBuiltIn = new System.Windows.Forms.ToolStripComboBox();
            this.menuImageOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuImageExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplayFitToWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplayActualSize = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplayModeFree = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplayModeLocked = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuDisplayCentre = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuDisplayZoomReset = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplayZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplayZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // bitmapDisplay
            // 
            this.bitmapDisplay.BackgroundImage = global::CDS.Imaging.Demo.Properties.Resources.double_bubble;
            this.bitmapDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bitmapDisplay.Location = new System.Drawing.Point(0, 24);
            this.bitmapDisplay.Mode = CDS.Imaging.WinForms.BitmapDisplayMode.FitToWindowCentred;
            this.bitmapDisplay.Name = "bitmapDisplay";
            this.bitmapDisplay.Size = new System.Drawing.Size(800, 426);
            this.bitmapDisplay.TabIndex = 0;
            this.bitmapDisplay.PaintOver += new CDS.Imaging.WinForms.PaintOverEvent(this.bitmapDisplay_PaintOver);
            this.bitmapDisplay.PaintUnder += new CDS.Imaging.WinForms.PaintUnderEvent(this.bitmapDisplay_PaintUnder);
            this.bitmapDisplay.DisplayModeChanged += new CDS.Imaging.WinForms.ModeEventHandler(this.bitmapDisplay_DisplayModeChanged);
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuImage,
            this.menuDisplay});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(800, 24);
            this.menu.TabIndex = 7;
            this.menu.Text = "menuStrip1";
            // 
            // menuImage
            // 
            this.menuImage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuImageBuiltIn,
            this.menuImageOpen,
            this.toolStripSeparator1,
            this.menuImageExit});
            this.menuImage.Name = "menuImage";
            this.menuImage.Size = new System.Drawing.Size(52, 20);
            this.menuImage.Text = "Image";
            // 
            // menuImageBuiltIn
            // 
            this.menuImageBuiltIn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.menuImageBuiltIn.Name = "menuImageBuiltIn";
            this.menuImageBuiltIn.Size = new System.Drawing.Size(121, 23);
            this.menuImageBuiltIn.SelectedIndexChanged += new System.EventHandler(this.MenuImageBuiltIn_SelectedIndexChanged);
            // 
            // menuImageOpen
            // 
            this.menuImageOpen.Name = "menuImageOpen";
            this.menuImageOpen.Size = new System.Drawing.Size(181, 22);
            this.menuImageOpen.Text = "Open";
            this.menuImageOpen.Click += new System.EventHandler(this.MenuImageOpen_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // menuImageExit
            // 
            this.menuImageExit.Name = "menuImageExit";
            this.menuImageExit.Size = new System.Drawing.Size(181, 22);
            this.menuImageExit.Text = "Exit";
            this.menuImageExit.Click += new System.EventHandler(this.MenuImageExit_Click);
            // 
            // menuDisplay
            // 
            this.menuDisplay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDisplayFitToWindow,
            this.menuDisplayActualSize,
            this.menuDisplayModeFree,
            this.menuDisplayModeLocked,
            this.toolStripSeparator3,
            this.menuDisplayCentre,
            this.toolStripSeparator2,
            this.menuDisplayZoomReset,
            this.menuDisplayZoomIn,
            this.menuDisplayZoomOut});
            this.menuDisplay.Name = "menuDisplay";
            this.menuDisplay.Size = new System.Drawing.Size(57, 20);
            this.menuDisplay.Text = "Display";
            // 
            // menuDisplayFitToWindow
            // 
            this.menuDisplayFitToWindow.Name = "menuDisplayFitToWindow";
            this.menuDisplayFitToWindow.Size = new System.Drawing.Size(181, 22);
            this.menuDisplayFitToWindow.Text = "Fit to window";
            this.menuDisplayFitToWindow.Click += new System.EventHandler(this.menuDisplayModeFitToWindow_Click);
            // 
            // menuDisplayActualSize
            // 
            this.menuDisplayActualSize.Name = "menuDisplayActualSize";
            this.menuDisplayActualSize.Size = new System.Drawing.Size(181, 22);
            this.menuDisplayActualSize.Text = "Actual size (centred)";
            this.menuDisplayActualSize.Click += new System.EventHandler(this.menuDisplayModeActualSize_Click);
            // 
            // menuDisplayModeFree
            // 
            this.menuDisplayModeFree.Name = "menuDisplayModeFree";
            this.menuDisplayModeFree.Size = new System.Drawing.Size(181, 22);
            this.menuDisplayModeFree.Text = "Free";
            this.menuDisplayModeFree.Click += new System.EventHandler(this.menuDisplayModeFree_Click);
            // 
            // menuDisplayModeLocked
            // 
            this.menuDisplayModeLocked.Name = "menuDisplayModeLocked";
            this.menuDisplayModeLocked.Size = new System.Drawing.Size(181, 22);
            this.menuDisplayModeLocked.Text = "Locked";
            this.menuDisplayModeLocked.Click += new System.EventHandler(this.menuDisplayModeLocked_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(178, 6);
            // 
            // menuDisplayCentre
            // 
            this.menuDisplayCentre.Name = "menuDisplayCentre";
            this.menuDisplayCentre.Size = new System.Drawing.Size(181, 22);
            this.menuDisplayCentre.Text = "Centre";
            this.menuDisplayCentre.Click += new System.EventHandler(this.menuDisplayCentre_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(178, 6);
            // 
            // menuDisplayZoomReset
            // 
            this.menuDisplayZoomReset.Name = "menuDisplayZoomReset";
            this.menuDisplayZoomReset.Size = new System.Drawing.Size(181, 22);
            this.menuDisplayZoomReset.Text = "Zoom 1:1";
            // 
            // menuDisplayZoomIn
            // 
            this.menuDisplayZoomIn.Name = "menuDisplayZoomIn";
            this.menuDisplayZoomIn.Size = new System.Drawing.Size(181, 22);
            this.menuDisplayZoomIn.Text = "Zoom in";
            // 
            // menuDisplayZoomOut
            // 
            this.menuDisplayZoomOut.Name = "menuDisplayZoomOut";
            this.menuDisplayZoomOut.Size = new System.Drawing.Size(181, 22);
            this.menuDisplayZoomOut.Text = "Zoom out";
            // 
            // FormBitmapDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.bitmapDisplay);
            this.Controls.Add(this.menu);
            this.MainMenuStrip = this.menu;
            this.Name = "FormBitmapDisplay";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormBitmapDisplay_Load);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
        #endregion

        private WinForms.BitmapDisplay bitmapDisplay;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem menuImage;
        private System.Windows.Forms.ToolStripMenuItem menuImageOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuImageExit;
        private System.Windows.Forms.ToolStripMenuItem menuDisplay;
        private System.Windows.Forms.ToolStripComboBox menuImageBuiltIn;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayFitToWindow;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayActualSize;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayModeFree;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayModeLocked;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayCentre;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayZoomReset;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayZoomIn;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayZoomOut;
    }
}

