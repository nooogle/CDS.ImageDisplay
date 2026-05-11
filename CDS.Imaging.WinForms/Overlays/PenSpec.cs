using CDS.Imaging.Utils;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json.Serialization;

namespace CDS.Imaging.Overlays;

/// <summary>
/// Represents the specification of a pen.
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class PenSpec
{
    /// <summary>
    /// Simple string representation of this instance.
    /// </summary>
    public override string ToString() => $"Color {Color}, Width {Width}, DashStyle {DashStyle}, StartCap {StartCap}, EndCap {EndCap}";


    /// <summary>
    /// The color of the pen.
    /// </summary>
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Color { get; set; } = Color.Red;


    /// <summary>
    /// The width of the pen.
    /// </summary>
    public float Width { get; set; } = 1;


    /// <summary>
    /// The dash style of the pen.
    /// </summary>
    public DashStyle DashStyle { get; set; } = DashStyle.Solid;


    /// <summary>
    /// The start cap of the pen.
    /// </summary>
    public LineCap StartCap { get; set; } = LineCap.Flat;


    /// <summary>
    /// The end cap of the pen.
    /// </summary>
    public LineCap EndCap { get; set; } = LineCap.Flat;


    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Color, Width, DashStyle, StartCap, EndCap);
    }


    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not PenSpec other) return false;

        return Color == other.Color &&
               Width == other.Width &&
               DashStyle == other.DashStyle &&
               StartCap == other.StartCap &&
               EndCap == other.EndCap;
    }


    /// <summary>
    /// Creates a pen from this specification.
    /// </summary>
    public Pen Create()
    {
        return new Pen(Color, Width)
        {
            DashStyle = DashStyle,
            StartCap = StartCap,
            EndCap = EndCap
        };
    }
}
