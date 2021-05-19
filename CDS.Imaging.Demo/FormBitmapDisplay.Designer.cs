
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBitmapDisplay));
            this.bitmapDisplay = new CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayPanel();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.menuImage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuImageBuiltIn = new System.Windows.Forms.ToolStripComboBox();
            this.menuImageOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuImageExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplayModeFitToWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplayModeActualSize = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplayModeFree = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuDisplayCentre = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuDisplayZoomReset = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplayZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplayZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.openImageDialog = new System.Windows.Forms.OpenFileDialog();
            this.crossHair1 = new CDS.Imaging.WinForms.Draw.SimpleCrossHair(this.components);
            this.menuDisplayFitToWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDisplayActualSize = new System.Windows.Forms.ToolStripMenuItem();
            this.menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // bitmapDisplay
            // 
            this.bitmapDisplay.BackgroundImage = global::CDS.Imaging.Demo.Properties.Resources.double_bubble;
            this.bitmapDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bitmapDisplay.TargetImageCentre = ((System.Drawing.PointF)(resources.GetObject("bitmapDisplay.ImageDisplayCentre")));
            this.bitmapDisplay.Location = new System.Drawing.Point(0, 27);
            this.bitmapDisplay.Margin = new System.Windows.Forms.Padding(6);
            this.bitmapDisplay.DisplayMode = CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplayMode.FitToWindowCentred;
            this.bitmapDisplay.Name = "bitmapDisplay";
            this.bitmapDisplay.Size = new System.Drawing.Size(624, 414);
            this.bitmapDisplay.TabIndex = 0;
            this.bitmapDisplay.Zoom = 1F;
            this.bitmapDisplay.PaintOver += new CDS.Imaging.WinForms.BitmapDisplay.PaintOverEvent(this.bitmapDisplay_PaintOver);
            this.bitmapDisplay.PaintUnder += new CDS.Imaging.WinForms.BitmapDisplay.PaintUnderEvent(this.bitmapDisplay_PaintUnder);
            this.bitmapDisplay.DisplayModeChanged += new CDS.Imaging.WinForms.BitmapDisplay.ModeChangedEvent(this.bitmapDisplay_DisplayModeChanged);
            // 
            // menu
            // 
            this.menu.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuImage,
            this.menuDisplay});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Padding = new System.Windows.Forms.Padding(11, 4, 0, 4);
            this.menu.Size = new System.Drawing.Size(624, 27);
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
            this.menuImage.Size = new System.Drawing.Size(60, 19);
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
            this.menuImageOpen.Size = new System.Drawing.Size(240, 44);
            this.menuImageOpen.Text = "Open";
            this.menuImageOpen.Click += new System.EventHandler(this.MenuImageOpen_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(237, 6);
            // 
            // menuImageExit
            // 
            this.menuImageExit.Name = "menuImageExit";
            this.menuImageExit.Size = new System.Drawing.Size(240, 44);
            this.menuImageExit.Text = "Exit";
            this.menuImageExit.Click += new System.EventHandler(this.MenuImageExit_Click);
            // 
            // menuDisplay
            // 
            this.menuDisplay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDisplayModeFitToWindow,
            this.menuDisplayModeActualSize,
            this.menuDisplayModeFree,
            this.toolStripSeparator3,
            this.menuDisplayCentre,
            this.menuDisplayActualSize,
            this.menuDisplayFitToWindow,
            this.toolStripSeparator2,
            this.menuDisplayZoomReset,
            this.menuDisplayZoomIn,
            this.menuDisplayZoomOut});
            this.menuDisplay.Name = "menuDisplay";
            this.menuDisplay.Size = new System.Drawing.Size(65, 19);
            this.menuDisplay.Text = "Display";
            // 
            // menuDisplayModeFitToWindow
            // 
            this.menuDisplayModeFitToWindow.Name = "menuDisplayModeFitToWindow";
            this.menuDisplayModeFitToWindow.Size = new System.Drawing.Size(255, 44);
            this.menuDisplayModeFitToWindow.Text = "Fit to window";
            this.menuDisplayModeFitToWindow.Click += new System.EventHandler(this.menuDisplayModeFitToWindow_Click);
            // 
            // menuDisplayModeActualSize
            // 
            this.menuDisplayModeActualSize.Name = "menuDisplayModeActualSize";
            this.menuDisplayModeActualSize.Size = new System.Drawing.Size(255, 44);
            this.menuDisplayModeActualSize.Text = "Actual size (centred)";
            this.menuDisplayModeActualSize.Click += new System.EventHandler(this.menuDisplayModeActualSize_Click);
            // 
            // menuDisplayModeFree
            // 
            this.menuDisplayModeFree.Name = "menuDisplayModeFree";
            this.menuDisplayModeFree.Size = new System.Drawing.Size(255, 44);
            this.menuDisplayModeFree.Text = "Free";
            this.menuDisplayModeFree.Click += new System.EventHandler(this.menuDisplayModeFree_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(252, 6);
            // 
            // menuDisplayCentre
            // 
            this.menuDisplayCentre.Name = "menuDisplayCentre";
            this.menuDisplayCentre.Size = new System.Drawing.Size(255, 44);
            this.menuDisplayCentre.Text = "Centre";
            this.menuDisplayCentre.Click += new System.EventHandler(this.menuDisplayCentre_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(252, 6);
            // 
            // menuDisplayZoomReset
            // 
            this.menuDisplayZoomReset.Name = "menuDisplayZoomReset";
            this.menuDisplayZoomReset.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D0)));
            this.menuDisplayZoomReset.Size = new System.Drawing.Size(255, 44);
            this.menuDisplayZoomReset.Text = "Zoom 1:1";
            this.menuDisplayZoomReset.Click += new System.EventHandler(this.menuDisplayZoomReset_Click);
            // 
            // menuDisplayZoomIn
            // 
            this.menuDisplayZoomIn.Name = "menuDisplayZoomIn";
            this.menuDisplayZoomIn.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.menuDisplayZoomIn.Size = new System.Drawing.Size(255, 44);
            this.menuDisplayZoomIn.Text = "Zoom in";
            this.menuDisplayZoomIn.Click += new System.EventHandler(this.menuDisplayZoomIn_Click);
            // 
            // menuDisplayZoomOut
            // 
            this.menuDisplayZoomOut.Name = "menuDisplayZoomOut";
            this.menuDisplayZoomOut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.menuDisplayZoomOut.Size = new System.Drawing.Size(255, 44);
            this.menuDisplayZoomOut.Text = "Zoom out";
            this.menuDisplayZoomOut.Click += new System.EventHandler(this.menuDisplayZoomOut_Click);
            // 
            // openImageDialog
            // 
            this.openImageDialog.Filter = "Bitmaps|*.bmp|JPEGs|*.jpg|TIFFs|*.tif|PNGs|*.png";
            // 
            // crossHair1
            // 
            this.crossHair1.CentreGap = 3F;
            this.crossHair1.Diameter = 20F;
            // 
            // 
            // 
            this.crossHair1.EllipsePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.crossHair1.EllipsePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            this.crossHair1.EllipsePen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            this.crossHair1.EllipsePen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            this.crossHair1.EllipsePen.Width = 3F;
            // 
            // 
            // 
            this.crossHair1.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.crossHair1.LinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            this.crossHair1.LinePen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            this.crossHair1.LinePen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            this.crossHair1.LinePen.Width = 2F;
            // 
            // menuDisplayFitToWindow
            // 
            this.menuDisplayFitToWindow.Name = "menuDisplayFitToWindow";
            this.menuDisplayFitToWindow.Size = new System.Drawing.Size(251, 44);
            this.menuDisplayFitToWindow.Text = "Fit to window";
            this.menuDisplayFitToWindow.Click += new System.EventHandler(MenuDisplayFitToWindow_Click);
            // 
            // menuDisplayActualSize
            // 
            this.menuDisplayActualSize.Name = "menuDisplayActualSize";
            this.menuDisplayActualSize.Size = new System.Drawing.Size(255, 44);
            this.menuDisplayActualSize.Text = "Actual size and centred";
            this.menuDisplayActualSize.Click += new System.EventHandler(MenuDisplayActualSize_Click);
            // 
            // FormBitmapDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.bitmapDisplay);
            this.Controls.Add(this.menu);
            this.MainMenuStrip = this.menu;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "FormBitmapDisplay";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormBitmapDisplay_Load);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplay;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem menuImage;
        private System.Windows.Forms.ToolStripMenuItem menuImageOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuImageExit;
        private System.Windows.Forms.ToolStripMenuItem menuDisplay;
        private System.Windows.Forms.ToolStripComboBox menuImageBuiltIn;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayModeFitToWindow;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayModeActualSize;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayModeFree;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayCentre;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayZoomReset;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayZoomIn;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayZoomOut;
        private System.Windows.Forms.OpenFileDialog openImageDialog;
        private WinForms.Draw.SimpleCrossHair crossHair1;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayActualSize;
        private System.Windows.Forms.ToolStripMenuItem menuDisplayFitToWindow;
    }
}

