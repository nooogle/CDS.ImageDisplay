
namespace CDS.Imaging.Demo
{
    partial class FormTestLauncher
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
            menuTree = new CDS.WinFormsMenus.Basic.MenuTree();
            sysInfoPanel = new Utils.SystemInfoPanel();
            SuspendLayout();
            //
            // menuTree
            //
            menuTree.Dock = System.Windows.Forms.DockStyle.Fill;
            menuTree.Location = new System.Drawing.Point(0, 38);
            menuTree.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            menuTree.Name = "menuTree";
            menuTree.Size = new System.Drawing.Size(509, 361);
            menuTree.TabIndex = 0;
            //
            // sysInfoPanel
            //
            sysInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            sysInfoPanel.Location = new System.Drawing.Point(0, 0);
            sysInfoPanel.Name = "sysInfoPanel";
            sysInfoPanel.Size = new System.Drawing.Size(509, 38);
            sysInfoPanel.TabIndex = 8;
            //
            // FormTestLauncher
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(509, 399);
            Controls.Add(menuTree);
            Controls.Add(sysInfoPanel);
            Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            Name = "FormTestLauncher";
            Text = "FormTestLauncher";
            Load += FormTestLauncher_Load;
            ResumeLayout(false);
        }

        #endregion

        private CDS.WinFormsMenus.Basic.MenuTree menuTree;
        private Utils.SystemInfoPanel sysInfoPanel;
    }
}