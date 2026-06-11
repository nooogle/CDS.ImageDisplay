using System.Drawing;

namespace CDS.ImageDisplay.WinForms.Overlays;

/// <summary>
/// A ready-to-use text panel that uses <see cref="TextPanelStdMsgTypes"/> and
/// <see cref="TextPanelStdMsgDrawingSpecs"/> so callers do not need to wire them up manually.
/// </summary>
/// <remarks>
/// For the common case, create one instance, call <see cref="AddMessage"/> for each line, and
/// call <see cref="Draw"/> from a <see cref="BitmapDisplay.BitmapDisplayPanel"/> paint event.
/// Customise colours and fonts via <see cref="DrawingSpecs"/> after construction.
/// To update messages on every paint, call <see cref="Clear"/> before adding new ones.
/// For custom message types, use <see cref="TextPanel{TMessageType}"/> directly.
/// </remarks>
public class TextPanelStd
{
    private readonly TextPanel<TextPanelStdMsgTypes> _panel = new();

    /// <summary>
    /// Gets the drawing specifications used to style the panel background and each message type.
    /// Modify these properties to override the default colours and fonts.
    /// </summary>
    public TextPanelStdMsgDrawingSpecs DrawingSpecs { get; } = new();

    /// <summary>
    /// Adds a message of the specified type.
    /// </summary>
    public void AddMessage(TextPanelStdMsgTypes type, string text)
        => _panel.AddMessage(type, text);

    /// <summary>
    /// Adds a message using white-space padding between two fields.
    /// </summary>
    public void AddPaddedMessage(TextPanelStdMsgTypes type, string text1, int text1PaddedLength, string text2)
        => _panel.AddPaddedMessage(type, text1, text1PaddedLength, text2);

    /// <summary>
    /// Adds a message using white-space padding between three fields.
    /// </summary>
    public void AddPaddedMessage(TextPanelStdMsgTypes type, string text1, int text1PaddedLength, string text2, int text2PaddedLength, string text3)
        => _panel.AddPaddedMessage(type, text1, text1PaddedLength, text2, text2PaddedLength, text3);

    /// <summary>
    /// Adds a blank separator line.
    /// </summary>
    public void AddBlankLine()
        => _panel.AddBlankLine();

    /// <summary>
    /// Removes all messages. Call this at the start of each paint event when messages change frame-to-frame.
    /// </summary>
    public void Clear()
        => _panel.Clear();

    /// <summary>
    /// Draws the panel onto <paramref name="bitmapDisplay"/> using <paramref name="graphics"/>.
    /// </summary>
    public void Draw(BitmapDisplay.BitmapDisplayPanel bitmapDisplay, Graphics graphics)
        => _panel.Draw(bitmapDisplay, graphics, DrawingSpecs.Panel, DrawingSpecs.GetDrawingSpecForMessageType);
}
