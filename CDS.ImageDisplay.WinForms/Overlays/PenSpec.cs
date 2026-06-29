using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json.Serialization;
using CDS.ImageDisplay.WinForms.Utils;


namespace CDS.ImageDisplay.WinForms.Overlays;

/// <summary>
/// Represents the specification of a pen.
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class PenSpec
{
    private Color _color = Color.Red;
    private float _width = 1;
    private DashStyle _dashStyle = DashStyle.Solid;
    private LineCap _startCap = LineCap.Flat;
    private LineCap _endCap = LineCap.Flat;

    /// <summary>Raised when any property of this specification changes.</summary>
    public event EventHandler? Changed;

    /// <summary>
    /// Simple string representation of this instance.
    /// </summary>
    public override string ToString() => $"Color {Color}, Width {Width}, DashStyle {DashStyle}, StartCap {StartCap}, EndCap {EndCap}";


    /// <summary>
    /// The color of the pen.
    /// </summary>
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Color
    {
        get => _color;
        set { if (_color == value) { return; } _color = value; Changed?.Invoke(this, EventArgs.Empty); }
    }


    /// <summary>
    /// The width of the pen.
    /// </summary>
    public float Width
    {
        get => _width;
        set { if (_width == value) { return; } _width = value; Changed?.Invoke(this, EventArgs.Empty); }
    }


    /// <summary>
    /// The dash style of the pen.
    /// </summary>
    public DashStyle DashStyle
    {
        get => _dashStyle;
        set { if (_dashStyle == value) { return; } _dashStyle = value; Changed?.Invoke(this, EventArgs.Empty); }
    }


    /// <summary>
    /// The start cap of the pen.
    /// </summary>
    public LineCap StartCap
    {
        get => _startCap;
        set { if (_startCap == value) { return; } _startCap = value; Changed?.Invoke(this, EventArgs.Empty); }
    }


    /// <summary>
    /// The end cap of the pen.
    /// </summary>
    public LineCap EndCap
    {
        get => _endCap;
        set { if (_endCap == value) { return; } _endCap = value; Changed?.Invoke(this, EventArgs.Empty); }
    }


    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = (hash * 31) + Color.GetHashCode();
            hash = (hash * 31) + Width.GetHashCode();
            hash = (hash * 31) + DashStyle.GetHashCode();
            hash = (hash * 31) + StartCap.GetHashCode();
            hash = (hash * 31) + EndCap.GetHashCode();
            return hash;
        }
    }


    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is PenSpec other && Color == other.Color &&
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

    /// <summary>
    /// Returns a shallow copy suitable for use as a stable dictionary key.
    /// All fields are value types so a shallow copy is a complete independent copy.
    /// </summary>
    internal PenSpec Clone() => (PenSpec)MemberwiseClone();
}
