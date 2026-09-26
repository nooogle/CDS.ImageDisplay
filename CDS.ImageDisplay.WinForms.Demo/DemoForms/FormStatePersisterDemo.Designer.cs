namespace CDS.ImageDisplay.WinForms.Demo.DemoForms
{
    partial class FormStatePersisterDemo
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
            label1 = new System.Windows.Forms.Label();
            formStatePersister = new CDS.ImageDisplay.WinForms.Utils.FormStatePersister(components);
            ((System.ComponentModel.ISupportInitialize)formStatePersister).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Dock = System.Windows.Forms.DockStyle.Fill;
            label1.Location = new System.Drawing.Point(0, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(495, 362);
            label1.TabIndex = 3;
            label1.Text = "This form saves and restores the position, size, minimised/maximised state, and screen affinity";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // formStatePersister
            // 
            formStatePersister.Form = this;
            // 
            // FormStatePersisterDemo
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(495, 362);
            Controls.Add(label1);
            Name = "FormStatePersisterDemo";
            Text = "FormStatePersisterDemo";
            ((System.ComponentModel.ISupportInitialize)formStatePersister).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Utils.FormStatePersister formStatePersister;
    }
}