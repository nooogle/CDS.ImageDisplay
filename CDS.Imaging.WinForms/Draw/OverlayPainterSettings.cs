using Newtonsoft.Json;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CDS.Imaging.WinForms.Draw
{
    /// <summary>
    /// Represents the settings for drawing overlays.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class OverlayPainterSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether drawing is enabled.
        /// </summary>
        /// <value><c>true</c> to enable drawing; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the pen color.
        /// </summary>
        /// <value>The color of the pen.</value>
        [DisplayName("Pen color")]
        public Color PenColor { get; set; } = Color.Lime;

        /// <summary>
        /// Gets or sets the pen dash style.
        /// </summary>
        /// <value>The dash style of the pen.</value>
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        [DisplayName("Pen dash style")]
        public DashStyle PenDashStyle { get; set; } = DashStyle.Solid;

        /// <summary>
        /// Gets or sets the pen start cap.
        /// </summary>
        /// <value>The start cap of the pen.</value>
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        [DisplayName("Pen start cap")]
        public LineCap PenStartCap { get; set; } = LineCap.NoAnchor;

        /// <summary>
        /// Gets or sets the pen end cap.
        /// </summary>
        /// <value>The end cap of the pen.</value>
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        [DisplayName("Pen end cap")]
        public LineCap PenEndCap { get; set; } = LineCap.NoAnchor;

        /// <summary>
        /// Gets or sets the pen width.
        /// </summary>
        /// <value>The width of the pen.</value>
        [DisplayName("Pen width")]
        public int PenWidth { get; set; }

        /// <summary>
        /// Gets or sets the brush color.
        /// </summary>
        /// <value>The color of the brush.</value>
        [DisplayName("Brush color")]
        public Color BrushColor { get; set; } = Color.Transparent;

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        /// <value>The size of the font.</value>
        [DisplayName("Font size")]
        public int FontSize { get; set; } = 12;

        /// <summary>
        /// Gets or sets the font name.
        /// </summary>
        /// <value>The name of the font.</value>
        [DisplayName("Font name")]
        public string FontName { get; set; } = "Arial";

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => Enabled ? $"Pen {PenColor}, Brush {BrushColor}" : "Disabled";
    }
}

