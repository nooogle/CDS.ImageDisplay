namespace CDS.Imaging.WinForms.Draw
{
    partial class OverlayPainter
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pen = new SimplePen(components);
            brush = new SimpleSolidBrush(components);
            // 
            // pen
            // 
            pen.Color = System.Drawing.Color.WhiteSmoke;
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
            pen.Width = 1F;
            // 
            // brush
            // 
            brush.Color = System.Drawing.Color.Yellow;
        }

        #endregion

        private SimplePen pen;
        private SimpleSolidBrush brush;
    }
}
