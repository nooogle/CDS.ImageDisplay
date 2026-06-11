namespace CDS.ImageDisplay.WinForms.Demo.DemoForms
{
    partial class FormNoImage
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
            bitmapDisplayPanel1 = new CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            bitmapDisplayPanel2 = new CDS.ImageDisplay.WinForms.BitmapDisplay.BitmapDisplayPanel();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            formStatePersister1 = new CDS.ImageDisplay.WinForms.Utils.FormStatePersister(components);
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // bitmapDisplayPanel1
            // 
            bitmapDisplayPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapDisplayPanel1.Location = new System.Drawing.Point(4, 58);
            bitmapDisplayPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            bitmapDisplayPanel1.Name = "bitmapDisplayPanel1";
            bitmapDisplayPanel1.Size = new System.Drawing.Size(1101, 292);
            bitmapDisplayPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(bitmapDisplayPanel2, 0, 3);
            tableLayoutPanel1.Controls.Add(bitmapDisplayPanel1, 0, 1);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 2);
            tableLayoutPanel1.Location = new System.Drawing.Point(17, 20);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 53F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 53F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new System.Drawing.Size(1109, 710);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // bitmapDisplayPanel2
            // 
            bitmapDisplayPanel2.BackgroundImage = Properties.Resources.double_bubble;
            bitmapDisplayPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            bitmapDisplayPanel2.Location = new System.Drawing.Point(4, 413);
            bitmapDisplayPanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            bitmapDisplayPanel2.Name = "bitmapDisplayPanel2";
            bitmapDisplayPanel2.Size = new System.Drawing.Size(1101, 292);
            bitmapDisplayPanel2.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = System.Windows.Forms.DockStyle.Left;
            label1.Location = new System.Drawing.Point(4, 0);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(408, 53);
            label1.TabIndex = 2;
            label1.Text = "No background image or runtime image assigned";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = System.Windows.Forms.DockStyle.Left;
            label2.Location = new System.Drawing.Point(4, 355);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(475, 53);
            label2.TabIndex = 3;
            label2.Text = "Background image applied via designer, no runtime image";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // formStatePersister1
            // 
            formStatePersister1.Form = this;
            // 
            // FormNoImage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1143, 750);
            Controls.Add(tableLayoutPanel1);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "FormNoImage";
            Text = "FormNoImageg";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private WinForms.BitmapDisplay.BitmapDisplayPanel bitmapDisplayPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Utils.FormStatePersister formStatePersister1;
    }
}