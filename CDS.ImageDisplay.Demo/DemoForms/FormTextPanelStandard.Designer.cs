namespace CDS.ImageDisplay.Demo.DemoForms
{
    partial class FormTextPanelStandard
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
            bitmapDisplayPanelStandard = new CDS.ImageDisplay.BitmapDisplay.BitmapDisplayPanel();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            label1 = new System.Windows.Forms.Label();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // bitmapDisplayPanelStandard
            // 
            bitmapDisplayPanelStandard.BackgroundImage = Properties.Resources.double_bubble;
            bitmapDisplayPanelStandard.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapDisplayPanelStandard.Location = new System.Drawing.Point(3, 35);
            bitmapDisplayPanelStandard.Name = "bitmapDisplayPanelStandard";
            bitmapDisplayPanelStandard.Size = new System.Drawing.Size(613, 285);
            bitmapDisplayPanelStandard.TabIndex = 0;
            bitmapDisplayPanelStandard.OnPaintOver += bitmapDisplayPanelStandard_OnPaintOver;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(bitmapDisplayPanelStandard, 0, 1);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new System.Drawing.Size(619, 323);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = System.Windows.Forms.DockStyle.Left;
            label1.Location = new System.Drawing.Point(3, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(261, 32);
            label1.TabIndex = 2;
            label1.Text = "Text panel using built-in type and drawing specs";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormTextPanelStandard
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(643, 347);
            Controls.Add(tableLayoutPanel1);
            Name = "FormTextPanelStandard";
            Text = "FormTextPanelStandard";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanelStandard;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
    }
}