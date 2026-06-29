using System;
using System.ComponentModel;
using CDS.ImageDisplay.WinForms.Utils;


namespace CDS.ImageDisplay.WinForms.Overlays;

/// <summary>
/// Bundles together the settings for lines, fills, and fonts.
/// </summary>
[TypeConverter(typeof(SerializableExpandableObjectConverter))]
public class DrawingSpec
{
    private bool _visible = true;
    private MappingMode _mappingMode = MappingMode.ImageToDisplay;
    private PenSpec _lines = new();
    private BrushSpec _fill = new();
    private FontSpec _font = new();

    /// <summary>
    /// Raised when any property of this specification, or any sub-specification, changes.
    /// </summary>
    public event EventHandler? Changed;

    /// <summary>Initializes a new instance of <see cref="DrawingSpec"/>.</summary>
    public DrawingSpec()
    {
        _lines.Changed += OnSubSpecChanged;
        _fill.Changed += OnSubSpecChanged;
        _font.Changed += OnSubSpecChanged;
    }

    /// <summary>
    /// Simple representation of the specification.
    /// </summary>
    public override string ToString() => $"Visible={Visible}, Mode={MappingMode}";


    /// <summary>
    /// True if the shape should be visible.
    /// </summary>
    public bool Visible
    {
        get => _visible;
        set { if (_visible == value) { return; } _visible = value; Changed?.Invoke(this, EventArgs.Empty); }
    }


    /// <summary>
    /// Line specification.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public PenSpec Lines
    {
        get => _lines;
        set
        {
            if (_lines == value) { return; }
            _lines.Changed -= OnSubSpecChanged;
            _lines = value ?? throw new ArgumentNullException(nameof(value));
            _lines.Changed += OnSubSpecChanged;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }


    /// <summary>
    /// Fill specification.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public BrushSpec Fill
    {
        get => _fill;
        set
        {
            if (_fill == value) { return; }
            _fill.Changed -= OnSubSpecChanged;
            _fill = value ?? throw new ArgumentNullException(nameof(value));
            _fill.Changed += OnSubSpecChanged;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }


    /// <summary>
    /// Font specification.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public FontSpec Font
    {
        get => _font;
        set
        {
            if (_font == value) { return; }
            _font.Changed -= OnSubSpecChanged;
            _font = value ?? throw new ArgumentNullException(nameof(value));
            _font.Changed += OnSubSpecChanged;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }


    /// <summary>
    /// How to map coordinates onto the display. Use <see cref="MappingMode.ImageToDisplay"/> when
    /// you want the graphics to shift and scale with the image. Use <see cref="MappingMode.DirectToDisplay"/>
    /// when you want the graphics to remain fixed on the display, regardless of the image position and zoom.
    /// </summary>
    [Description(
        "How to map coordinates onto the display. Use ImageToDisplay when you want the " +
        "graphics to shift and scale with the image. Use DisplayToImage when you " +
        "want the graphics to remain fixed on the display, " +
        "regardless of the image position and zoom.")]
    public MappingMode MappingMode
    {
        get => _mappingMode;
        set { if (_mappingMode == value) { return; } _mappingMode = value; Changed?.Invoke(this, EventArgs.Empty); }
    }

    /// <summary>
    /// Copies all properties from <paramref name="source"/> into this instance.
    /// Fires <see cref="Changed"/> once per property that is modified.
    /// </summary>
    public void CopyFrom(DrawingSpec source)
    {
        if (source is null) { throw new ArgumentNullException(nameof(source)); }
        Visible = source.Visible;
        MappingMode = source.MappingMode;
        Lines = source.Lines.Clone();
        Fill = source.Fill.Clone();
        Font = source.Font.Clone();
    }

    private void OnSubSpecChanged(object? sender, EventArgs e) => Changed?.Invoke(this, EventArgs.Empty);
}
