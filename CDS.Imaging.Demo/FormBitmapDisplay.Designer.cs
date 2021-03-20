
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
            this.bitmapDisplay = new CDS.Imaging.WinForms.BitmapDisplay.BitmapDisplay();
            this.panelControlBox = new System.Windows.Forms.Panel();
            this.btnActualSize = new System.Windows.Forms.Button();
            this.btnFitToWindow = new System.Windows.Forms.Button();
            this.rbtnDisplayModeLocked = new System.Windows.Forms.RadioButton();
            this.rbtnDisplayModeFree = new System.Windows.Forms.RadioButton();
            this.rbtnDisplayModeActualSize = new System.Windows.Forms.RadioButton();
            this.rbtnDisplayModeFitToWindow = new System.Windows.Forms.RadioButton();
            this.btnCentre = new System.Windows.Forms.Button();
            this.panelControlBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // bitmapDisplay
            // 
            this.bitmapDisplay.BackgroundImage = global::CDS.Imaging.Demo.Properties.Resources.double_bubble;
            this.bitmapDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bitmapDisplay.Location = new System.Drawing.Point(0, 0);
            this.bitmapDisplay.Mode = CDS.Imaging.WinForms.BitmapDisplay.ImageDisplayMode.FitToWindowCentred;
            this.bitmapDisplay.Name = "bitmapDisplay";
            this.bitmapDisplay.Size = new System.Drawing.Size(800, 450);
            this.bitmapDisplay.TabIndex = 0;
            this.bitmapDisplay.PaintOver += new CDS.Imaging.WinForms.BitmapDisplay.PaintOverEvent(this.bitmapDisplay_PaintOver);
            // 
            // panelControlBox
            // 
            this.panelControlBox.BackColor = System.Drawing.SystemColors.Control;
            this.panelControlBox.Controls.Add(this.btnCentre);
            this.panelControlBox.Controls.Add(this.btnActualSize);
            this.panelControlBox.Controls.Add(this.btnFitToWindow);
            this.panelControlBox.Controls.Add(this.rbtnDisplayModeLocked);
            this.panelControlBox.Controls.Add(this.rbtnDisplayModeFree);
            this.panelControlBox.Controls.Add(this.rbtnDisplayModeActualSize);
            this.panelControlBox.Controls.Add(this.rbtnDisplayModeFitToWindow);
            this.panelControlBox.Location = new System.Drawing.Point(12, 12);
            this.panelControlBox.Name = "panelControlBox";
            this.panelControlBox.Size = new System.Drawing.Size(674, 44);
            this.panelControlBox.TabIndex = 6;
            this.panelControlBox.Text = "Display mode";
            // 
            // btnActualSize
            // 
            this.btnActualSize.AccessibleDescription = "";
            this.btnActualSize.Location = new System.Drawing.Point(431, 15);
            this.btnActualSize.Name = "btnActualSize";
            this.btnActualSize.Size = new System.Drawing.Size(107, 23);
            this.btnActualSize.TabIndex = 5;
            this.btnActualSize.Text = "Actual size";
            this.btnActualSize.UseVisualStyleBackColor = true;
            this.btnActualSize.Click += new System.EventHandler(this.btnActualSize_Click);
            // 
            // btnFitToWindow
            // 
            this.btnFitToWindow.AccessibleDescription = "";
            this.btnFitToWindow.Location = new System.Drawing.Point(318, 15);
            this.btnFitToWindow.Name = "btnFitToWindow";
            this.btnFitToWindow.Size = new System.Drawing.Size(107, 23);
            this.btnFitToWindow.TabIndex = 4;
            this.btnFitToWindow.Text = "Fit to window";
            this.btnFitToWindow.UseVisualStyleBackColor = true;
            this.btnFitToWindow.Click += new System.EventHandler(this.btnFitToWindow_Click);
            // 
            // rbtnDisplayModeLocked
            // 
            this.rbtnDisplayModeLocked.AutoSize = true;
            this.rbtnDisplayModeLocked.Location = new System.Drawing.Point(249, 19);
            this.rbtnDisplayModeLocked.Name = "rbtnDisplayModeLocked";
            this.rbtnDisplayModeLocked.Size = new System.Drawing.Size(63, 19);
            this.rbtnDisplayModeLocked.TabIndex = 3;
            this.rbtnDisplayModeLocked.Text = "Locked";
            this.rbtnDisplayModeLocked.UseVisualStyleBackColor = true;
            this.rbtnDisplayModeLocked.CheckedChanged += new System.EventHandler(this.rbtnDisplayModeLocked_CheckedChanged);
            // 
            // rbtnDisplayModeFree
            // 
            this.rbtnDisplayModeFree.AutoSize = true;
            this.rbtnDisplayModeFree.Checked = true;
            this.rbtnDisplayModeFree.Location = new System.Drawing.Point(196, 19);
            this.rbtnDisplayModeFree.Name = "rbtnDisplayModeFree";
            this.rbtnDisplayModeFree.Size = new System.Drawing.Size(47, 19);
            this.rbtnDisplayModeFree.TabIndex = 2;
            this.rbtnDisplayModeFree.TabStop = true;
            this.rbtnDisplayModeFree.Text = "Free";
            this.rbtnDisplayModeFree.UseVisualStyleBackColor = true;
            this.rbtnDisplayModeFree.CheckedChanged += new System.EventHandler(this.rbtnDisplayModeFree_CheckedChanged);
            // 
            // rbtnDisplayModeActualSize
            // 
            this.rbtnDisplayModeActualSize.AutoSize = true;
            this.rbtnDisplayModeActualSize.Location = new System.Drawing.Point(109, 19);
            this.rbtnDisplayModeActualSize.Name = "rbtnDisplayModeActualSize";
            this.rbtnDisplayModeActualSize.Size = new System.Drawing.Size(81, 19);
            this.rbtnDisplayModeActualSize.TabIndex = 1;
            this.rbtnDisplayModeActualSize.Text = "Actual size";
            this.rbtnDisplayModeActualSize.UseVisualStyleBackColor = true;
            this.rbtnDisplayModeActualSize.CheckedChanged += new System.EventHandler(this.rbtnDisplayModeActualSize_CheckedChanged);
            // 
            // rbtnDisplayModeFitToWindow
            // 
            this.rbtnDisplayModeFitToWindow.AutoSize = true;
            this.rbtnDisplayModeFitToWindow.Location = new System.Drawing.Point(6, 19);
            this.rbtnDisplayModeFitToWindow.Name = "rbtnDisplayModeFitToWindow";
            this.rbtnDisplayModeFitToWindow.Size = new System.Drawing.Size(97, 19);
            this.rbtnDisplayModeFitToWindow.TabIndex = 0;
            this.rbtnDisplayModeFitToWindow.Text = "Fit to window";
            this.rbtnDisplayModeFitToWindow.UseVisualStyleBackColor = true;
            this.rbtnDisplayModeFitToWindow.CheckedChanged += new System.EventHandler(this.rbtnDisplayModeFitToWindow_CheckedChanged);
            // 
            // btnCentre
            // 
            this.btnCentre.AccessibleDescription = "";
            this.btnCentre.Location = new System.Drawing.Point(544, 15);
            this.btnCentre.Name = "btnCentre";
            this.btnCentre.Size = new System.Drawing.Size(107, 23);
            this.btnCentre.TabIndex = 6;
            this.btnCentre.Text = "Centre";
            this.btnCentre.UseVisualStyleBackColor = true;
            this.btnCentre.Click += new System.EventHandler(this.btnCentre_Click);
            // 
            // FormBitmapDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panelControlBox);
            this.Controls.Add(this.bitmapDisplay);
            this.Name = "FormBitmapDisplay";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormBitmapDisplay_Load);
            this.panelControlBox.ResumeLayout(false);
            this.panelControlBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private WinForms.BitmapDisplay.BitmapDisplay bitmapDisplay;
        private System.Windows.Forms.Panel panelControlBox;
        private System.Windows.Forms.RadioButton rbtnDisplayModeLocked;
        private System.Windows.Forms.RadioButton rbtnDisplayModeFree;
        private System.Windows.Forms.RadioButton rbtnDisplayModeActualSize;
        private System.Windows.Forms.RadioButton rbtnDisplayModeFitToWindow;
        private System.Windows.Forms.Button btnActualSize;
        private System.Windows.Forms.Button btnFitToWindow;
        private System.Windows.Forms.Panel el;
        private System.Windows.Forms.Button btnCentre;
    }
}

