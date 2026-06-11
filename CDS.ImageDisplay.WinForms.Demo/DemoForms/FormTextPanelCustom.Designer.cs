namespace CDS.ImageDisplay.WinForms.Demo.DemoForms
{
    partial class FormTextPanelCustom
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
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            bitmapDisplayPanelCustom = new CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel();
            label2 = new System.Windows.Forms.Label();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(bitmapDisplayPanelCustom, 0, 1);
            tableLayoutPanel1.Controls.Add(label2, 0, 0);
            tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new System.Drawing.Size(650, 627);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // bitmapDisplayPanelCustom
            // 
            bitmapDisplayPanelCustom.BackgroundImage = Properties.Resources.double_bubble;
            bitmapDisplayPanelCustom.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapDisplayPanelCustom.Location = new System.Drawing.Point(3, 35);
            bitmapDisplayPanelCustom.Name = "bitmapDisplayPanelCustom";
            bitmapDisplayPanelCustom.Size = new System.Drawing.Size(644, 589);
            bitmapDisplayPanelCustom.TabIndex = 2;
            bitmapDisplayPanelCustom.PaintOver += bitmapDisplayPanelCustom_OnPaintOver;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = System.Windows.Forms.DockStyle.Left;
            label2.Location = new System.Drawing.Point(3, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(320, 32);
            label2.TabIndex = 3;
            label2.Text = "Text panel using a custom message type and drawing specs";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormTextPanelCustom
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(674, 651);
            Controls.Add(tableLayoutPanel1);
            Name = "FormTextPanelCustom";
            Text = "FormTextPanelCustom";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanelCustom;
        private System.Windows.Forms.Label label2;
    }
}