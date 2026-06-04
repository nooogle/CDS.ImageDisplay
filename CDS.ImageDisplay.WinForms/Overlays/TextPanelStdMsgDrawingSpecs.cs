using System;
using System.Drawing;

namespace CDS.ImageDisplay.Overlays;

/// <summary>
/// Represents the drawing specifications for standard message types in a text panel.
/// </summary>
public class TextPanelStdMsgDrawingSpecs
{
    /// <summary>
    /// Gets or sets the drawing specification for the panel.
    /// </summary>
    public DrawingSpec Panel { get; set; } = new();

    /// <summary>
    /// Gets or sets the drawing specification for title messages.
    /// </summary>
    public DrawingSpec Title { get; set; } = new();

    /// <summary>
    /// Gets or sets the drawing specification for informational messages.
    /// </summary>
    public DrawingSpec Info { get; set; } = new();

    /// <summary>
    /// Gets or sets the drawing specification for success messages.
    /// </summary>
    public DrawingSpec Success { get; set; } = new();

    /// <summary>
    /// Gets or sets the drawing specification for warning messages.
    /// </summary>
    public DrawingSpec Warning { get; set; } = new();

    /// <summary>
    /// Gets or sets the drawing specification for error messages.
    /// </summary>
    public DrawingSpec Error { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TextPanelStdMsgDrawingSpecs"/> class.
    /// </summary>
    public TextPanelStdMsgDrawingSpecs()
    {
        Panel.Fill.Color = Color.FromArgb(192, Color.Navy);
        Panel.Lines.Color = Color.Black;

        Title.Fill.Color = Color.WhiteSmoke;
        Title.Font.FontSize = 16;

        Info.Fill.Color = Color.WhiteSmoke;
        Info.Font.FontSize = 10;

        Success.Fill.Color = Color.LightGreen;
        Success.Font.FontSize = 10;

        Warning.Fill.Color = Color.LightGoldenrodYellow;
        Warning.Font.FontSize = 10;

        Error.Fill.Color = Color.LightCoral;
        Error.Font.FontSize = 10;
    }


    /// <summary>
    /// Gets the drawing specification for the specified message type.
    /// </summary>
    /// <param name="messageType">The type of the message.</param>
    /// <returns>The drawing specification for the specified message type.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the message type is not recognized.</exception>
    public DrawingSpec GetDrawingSpecForMessageType(TextPanelStdMsgTypes messageType)
    {
        return messageType switch
        {
            TextPanelStdMsgTypes.Title => Title,
            TextPanelStdMsgTypes.Info => Info,
            TextPanelStdMsgTypes.Success => Success,
            TextPanelStdMsgTypes.Warning => Warning,
            TextPanelStdMsgTypes.Error => Error,
            _ => throw new ArgumentOutOfRangeException(nameof(messageType)),
        };
    }
}
