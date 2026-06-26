using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using CDS.ImageDisplay.WinForms.BitmapDisplay;
using CDS.ImageDisplay.WinForms.Utils;


namespace CDS.ImageDisplay.WinForms.Overlays;


/// <summary>
/// A donut (or donut-slice) overlay shape. The ring is centred on an ellipse and drawn
/// between an inner and outer ellipse, each offset from the reference ellipse by half of
/// <see cref="Thickness"/>. When <see cref="SweepAngle"/> is 360 the shape is a full ring;
/// smaller values produce an arc slice.
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class DonutShape
{
    /// <summary>Simple representation of this instance.</summary>
    public override string ToString() =>
        SweepAngle >= 360f
            ? $"Donut: centre={Centre}, major={SemiMajorAxis}, minor={SemiMinorAxis}, thickness={Thickness}"
            : $"Donut slice: centre={Centre}, {StartAngle:0.#}°→{StartAngle + SweepAngle:0.#}°, thickness={Thickness}";


    /// <summary>
    /// Controls how the centre of the donut is aligned to the pixel grid.
    /// Only applicable when the mapping mode is <see cref="MappingMode.ImageToDisplay"/>.
    /// </summary>
    public DisplayPixelAlign PixelAlign { get; set; } = DisplayPixelAlign.Centre;


    /// <summary>The centre of the donut in image coordinates.</summary>
    [TypeConverter(typeof(PointFConverter))]
    public PointF Centre { get; set; }


    /// <summary>Semi-major axis of the reference ellipse in image coordinates.</summary>
    public float SemiMajorAxis { get; set; } = 50f;


    /// <summary>Semi-minor axis of the reference ellipse in image coordinates.</summary>
    public float SemiMinorAxis { get; set; } = 50f;


    /// <summary>Clockwise rotation of the major axis in degrees.</summary>
    public float MajorAxisAngleDegrees { get; set; }


    /// <summary>Angle at which the arc starts, in degrees clockwise from the positive x-axis.</summary>
    public float StartAngle { get; set; }


    /// <summary>
    /// Sweep of the arc in degrees. 360 produces a full ring; smaller values produce a slice.
    /// Negative values sweep counter-clockwise.
    /// </summary>
    public float SweepAngle { get; set; } = 360f;


    /// <summary>Radial thickness of the donut ring in image coordinates.</summary>
    public float Thickness { get; set; } = 10f;


    /// <summary>Draws the donut shape on the display.</summary>
    public void Draw(ICoordinateMapper mapper, Graphics graphics, DrawingSpec drawing)
    {
        if (mapper == null) { throw new ArgumentNullException(nameof(mapper)); }
        if (graphics == null) { throw new ArgumentNullException(nameof(graphics)); }
        if (drawing == null) { throw new ArgumentNullException(nameof(drawing)); }
        if (!drawing.Visible) { return; }
        if (SemiMajorAxis <= 0f || SemiMinorAxis <= 0f) { return; }
        if (SweepAngle == 0f) { return; }

        Pen pen = DrawingToolsPool.GetPen(drawing.Lines);
        Brush brush = DrawingToolsPool.GetBrush(drawing.Fill);

        float halfThick = Thickness / 2f;
        PointF centreDisplay;
        RectangleF outerRect, innerRect;

        if (drawing.MappingMode == MappingMode.ImageToDisplay)
        {
            float innerMajor = Math.Max(0f, SemiMajorAxis - halfThick);
            float innerMinor = Math.Max(0f, SemiMinorAxis - halfThick);

            RectangleF outerDisplay = mapper.MapRect(
                new RectangleF(Centre.X - (SemiMajorAxis + halfThick), Centre.Y - (SemiMinorAxis + halfThick),
                               (SemiMajorAxis + halfThick) * 2f, (SemiMinorAxis + halfThick) * 2f), PixelAlign);
            RectangleF innerDisplay = mapper.MapRect(
                new RectangleF(Centre.X - innerMajor, Centre.Y - innerMinor,
                               innerMajor * 2f, innerMinor * 2f), PixelAlign);

            centreDisplay = new PointF(outerDisplay.X + outerDisplay.Width / 2f,
                                       outerDisplay.Y + outerDisplay.Height / 2f);
            outerRect = new RectangleF(-outerDisplay.Width / 2f, -outerDisplay.Height / 2f,
                                        outerDisplay.Width, outerDisplay.Height);
            innerRect = new RectangleF(-innerDisplay.Width / 2f, -innerDisplay.Height / 2f,
                                        innerDisplay.Width, innerDisplay.Height);
        }
        else
        {
            centreDisplay = Centre;
            float outerMajor = SemiMajorAxis + halfThick;
            float outerMinor = SemiMinorAxis + halfThick;
            float innerMajor = Math.Max(0f, SemiMajorAxis - halfThick);
            float innerMinor = Math.Max(0f, SemiMinorAxis - halfThick);
            outerRect = new RectangleF(-outerMajor, -outerMinor, outerMajor * 2f, outerMinor * 2f);
            innerRect = new RectangleF(-innerMajor, -innerMinor, innerMajor * 2f, innerMinor * 2f);
        }

        GraphicsState state = graphics.Save();
        try
        {
            graphics.TranslateTransform(centreDisplay.X, centreDisplay.Y);
            graphics.RotateTransform(MajorAxisAngleDegrees);

            using var path = new GraphicsPath();
            path.AddArc(outerRect, StartAngle, SweepAngle);
            path.AddArc(innerRect, StartAngle + SweepAngle, -SweepAngle);
            path.CloseFigure();

            graphics.FillPath(brush, path);
            graphics.DrawPath(pen, path);
        }
        finally
        {
            graphics.Restore(state);
        }
    }
}
