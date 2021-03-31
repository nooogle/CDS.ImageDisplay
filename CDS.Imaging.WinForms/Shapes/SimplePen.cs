using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CDS.Imaging.WinForms.Shapes
{
    public partial class SimplePen : Component
    {
        /// <summary>
        /// The .Net Pen
        /// </summary>
        [Browsable(false)]
        public Pen Pen { get; private set; } = new(Color.Red);


        /// <summary>
        /// Gets or sets the color of this System.Drawing.Pen.
        /// </summary>
        public Color Color
        {
            get => Pen.Color;
            set => Pen.Color = value;
        }


        /// <summary>
        /// Gets or sets the width of this System.Drawing.Pen, in units of the System.Drawing.Graphics
        /// object used for drawing.
        /// </summary>
        public float Width
        {
            get => Pen.Width;
            set => Pen.Width = value;
        }


        /// <summary>
        /// Gets or sets the cap style used at the end of lines drawn with this System.Drawing.Pen.
        /// </summary>
        public LineCap EndCap
        {
            get => Pen.EndCap;
            set => Pen.EndCap = value;
        }


        /// <summary>
        /// Gets or sets the cap style used at the beginning of lines drawn with this System.Drawing.Pen.
        /// </summary>
        public LineCap StartCap
        {
            get => Pen.StartCap;
            set => Pen.StartCap = value;
        }


        /// <summary>
        /// Gets or sets the style used for dashed lines drawn with this System.Drawing.Pen.
        /// </summary>
        public DashStyle DashStyle
        {
            get => Pen.DashStyle;
            set => Pen.DashStyle = value;
        }



        public SimplePen()
        {
            InitializeComponent();
        }

        public SimplePen(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }


        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Pen?.Dispose();

                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }


        public static implicit operator Pen(SimplePen cPen) => cPen.Pen;
    }
}
