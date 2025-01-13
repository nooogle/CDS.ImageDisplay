using System.ComponentModel;
using System.Drawing;

namespace CDS.Imaging.WinForms.RegionOfInterest
{
    /// <summary>
    /// Renders a rectangle on a graphics object. Supports grapples.
    /// </summary>
    public partial class RectangleRenderer : Component
    {
        /// <summary>
        /// Specifies how the grapple points of the rectangle should be rendered.
        /// </summary>
        public enum GrapplesRenderingMode
        {
            /// <summary>
            /// The grapple points are hidden.
            /// </summary>
            Hide,


            /// <summary>
            /// The grapple points are shown only when the rectangle is enabled.
            /// </summary>
            ShowEnabled,


            /// <summary>
            /// The grapple points are shown only when the rectangle is disabled.
            /// </summary>
            ShowDisabled
        }


        private Draw.SimplePen? outlinePen;
        private Draw.SimpleSolidBrush? fillBrush;
        private Draw.SimplePen? enabledGrapplePen;
        private Draw.SimpleSolidBrush? enabledGrappleBrush;
        private Draw.SimplePen? disabledGrapplePen;
        private Draw.SimpleSolidBrush? disabledGrappleBrush;
        private Rectangle[] grappleRectangles = new Rectangle[8];


        /// <summary>
        /// Controls whether the rectangle is visible.
        /// </summary>
        public bool Visible { get; set; } = true;


        /// <summary>
        /// Gets or sets the rendering mode of the grapple points.
        /// </summary>
        public GrapplesRenderingMode GrapplesMode { get; set; } = GrapplesRenderingMode.ShowEnabled;




        /// <summary>
        /// The pen used to draw the outline of the rectangle.
        /// </summary>
        public Draw.SimplePen OutlinePen => outlinePen!;


        /// <summary>
        /// The pen used to draw the grapple points of the rectangle.
        /// </summary>
        public Draw.SimplePen EnabledGrapplePen => enabledGrapplePen!;


        /// <summary>
        /// The brush used to fill the rectangle.
        /// </summary>
        public Draw.SimpleSolidBrush FillBrush => fillBrush!;


        /// <summary>
        /// The brush used to fill the grapple points of the rectangle.
        /// </summary>
        public Draw.SimpleSolidBrush EnabledGrappleBrush => enabledGrappleBrush!;


        /// <summary>
        /// The pen used to draw the grapple points of the rectangle when it is disabled.
        /// </summary>
        public Draw.SimplePen DisabledGrapplePen => disabledGrapplePen!;


        /// <summary>
        /// The brush used to fill the grapple points of the rectangle when it is disabled.
        /// </summary>
        public Draw.SimpleSolidBrush DisabledGrappleBrush => disabledGrappleBrush!;


        /// <summary>
        /// The diameter of the grapple points.
        /// </summary>
        public int GrappleDiameter { get; set; } = 6;


        /// <summary>
        /// Default constructor.
        /// </summary>
        public RectangleRenderer()
        {
            InitializeComponent();
            CommonInitialise();
        }


        /// <summary>
        /// Constructor that takes a container.
        /// </summary>
        public RectangleRenderer(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            CommonInitialise();
        }


        /// <summary>
        /// Common initialisation for the constructors.
        /// </summary>
        private void CommonInitialise()
        {
            components.Add(outlinePen);
            components.Add(fillBrush);
            components.Add(enabledGrapplePen);
            components.Add(enabledGrappleBrush);
            components.Add(disabledGrapplePen);
            components.Add(disabledGrappleBrush);

            outlinePen = new Draw.SimplePen() { Color = Color.White };
            fillBrush = new Draw.SimpleSolidBrush() { Color = Color.Transparent };

            enabledGrapplePen = new Draw.SimplePen() { Color = Color.Navy };
            enabledGrappleBrush = new Draw.SimpleSolidBrush() { Color = Color.Navy };

            disabledGrapplePen = new Draw.SimplePen() { Color = Color.Gray };
            disabledGrappleBrush = new Draw.SimpleSolidBrush() { Color = Color.Gray };
        }


        /// <summary>
        /// Returns the pen and brush to use for rendering the grapple points.
        /// </summary>
        private (Draw.SimplePen? grapplesPen, Draw.SimpleSolidBrush? grapplesBrush) GetGrapplesPenAndBrush()
        {
            if (GrapplesMode == GrapplesRenderingMode.Hide)
            {
                return (null, null);
            }
            if (GrapplesMode == GrapplesRenderingMode.ShowEnabled)
            {
                return (enabledGrapplePen, enabledGrappleBrush);
            }

            return (disabledGrapplePen, disabledGrappleBrush);
        }


        /// <summary>
        /// Draws the rectangle on the specified graphics object.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rectangle"></param>
        public void Draw(Graphics graphics, Rectangle rectangle)
        {
            if(!Visible) { return; }
            if (rectangle.IsEmpty) { return; }
            if (graphics == null) { return; }
            if ((fillBrush == null) || (outlinePen == null)) { return; }

            (Draw.SimplePen? grapplesPen, Draw.SimpleSolidBrush? grapplesBrush) = GetGrapplesPenAndBrush();

            if (GrapplesMode != GrapplesRenderingMode.Hide)
            {
                RecalculateGrapplesRectangles(rectangle);
            }

            if (fillBrush.Color != Color.Transparent)
            {
                graphics.FillRectangle(fillBrush, rectangle);

                if (grapplesBrush != null)
                {
                    for (int grappleIndex = 0; grappleIndex < grappleRectangles.Length; grappleIndex++)
                    {
                        graphics.FillRectangle(grapplesBrush, grappleRectangles[grappleIndex]);
                    }
                }
            }

            if (outlinePen.Color != Color.Transparent)
            {
                graphics.DrawRectangle(outlinePen, rectangle);

                if (grapplesPen != null)
                {
                    for (int grappleIndex = 0; grappleIndex < grappleRectangles.Length; grappleIndex++)
                    {
                        graphics.DrawRectangle(grapplesPen, grappleRectangles[grappleIndex]);
                    }
                }
            }
        }


        /// <summary>
        /// Recalculates the rectangles for the grapple points.
        /// </summary>
        private void RecalculateGrapplesRectangles(Rectangle rectangle)
        {
            grappleRectangles[0] = CreateGrappleRect(location: rectangle.Location);
            grappleRectangles[1] = CreateGrappleRect(location: new Point(rectangle.Right, rectangle.Top));
            grappleRectangles[2] = CreateGrappleRect(location: new Point(rectangle.Right, rectangle.Bottom));
            grappleRectangles[3] = CreateGrappleRect(location: new Point(rectangle.Left, rectangle.Bottom));
            grappleRectangles[4] = CreateGrappleRect(location: new Point(rectangle.Left + rectangle.Width / 2, rectangle.Top));
            grappleRectangles[5] = CreateGrappleRect(location: new Point(rectangle.Right, rectangle.Top + rectangle.Height / 2));
            grappleRectangles[6] = CreateGrappleRect(location: new Point(rectangle.Left + rectangle.Width / 2, rectangle.Bottom));
            grappleRectangles[7] = CreateGrappleRect(location: new Point(rectangle.Left, rectangle.Top + rectangle.Height / 2));
        }


        /// <summary>
        /// Creates a rectangle for a grapple point.
        /// </summary>
        private Rectangle CreateGrappleRect(Point location)
        {
            return new Rectangle(
                x: location.X - GrappleDiameter / 2,
                y: location.Y - GrappleDiameter / 2,
                width: GrappleDiameter,
                height: GrappleDiameter);
        }
    }
}
