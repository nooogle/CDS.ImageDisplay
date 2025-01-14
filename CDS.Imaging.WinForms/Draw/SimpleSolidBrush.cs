using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.WinForms.Draw
{
    /// <summary>
    /// A simple solid brush; allows configuration within the designer
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public partial class SimpleSolidBrush : Component
    {
        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString() => "";


        /// <summary>
        /// The .Net Brush
        /// </summary>
        [Browsable(false)]
        public SolidBrush Brush { get; private set; } = new(Color.Red);


        /// <summary>
        /// Gets or sets the color of this brush.
        /// </summary>
        public Color Color
        {
            get => Brush.Color;
            set => Brush.Color = value;
        }
        

        /// <summary>
        /// Initialise
        /// </summary>
        public SimpleSolidBrush()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Initialise
        /// </summary>
        public SimpleSolidBrush(IContainer container)
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
                Brush?.Dispose();

                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }


        /// <summary>
        /// Concert to a .Net pen
        /// </summary>
        public static implicit operator Brush(SimpleSolidBrush simpleBrush) => simpleBrush.Brush;
    }
}
