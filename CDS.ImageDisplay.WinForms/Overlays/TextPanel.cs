using System;
using System.Collections.Generic;
using System.Drawing;

namespace CDS.ImageDisplay.WinForms.Overlays;


/// <summary>
/// Utility to present textual information on an bitmap display panel.
/// </summary>
public class TextPanel<TMessageType> where TMessageType : Enum
{
    /// <summary>
    /// A message line
    /// </summary>
    private record MessageLine(TMessageType? Type, string Text);


    /// <summary>The list of messages to draw.</summary>
    private readonly List<MessageLine> _messageLines = [];


    /// <summary>
    /// Adds a new message.
    /// </summary>
    public void AddMessage(TMessageType? type, string text)
    {
        _messageLines.Add(new MessageLine(type, text));
    }


    /// <summary>
    /// Adds a message using white-space padding.
    /// </summary>
    public void AddPaddedMessage(TMessageType type, string text1, int text1PaddedLength, string text2)
    {
        AddMessage(
            type,
            text1.PadRight(text1PaddedLength) + text2);
    }


    /// <summary>
    /// Adds a message using white-space padding.
    /// </summary>
    public void AddPaddedMessage(TMessageType type, string text1, int text1PaddedLength, string text2, int text2PaddedLength, string text3)
    {
        AddMessage(
            type,
            text1.PadRight(text1PaddedLength) + text2.PadRight(text2PaddedLength) + text3);
    }


    /// <summary>
    /// Adds a blank line.
    /// </summary>
    public void AddBlankLine()
    {
        AddMessage(default, " ");
    }


    /// <summary>
    /// Removes all messages.
    /// </summary>
    public void Clear()
    {
        _messageLines.Clear();
    }


    /// <summary>
    /// Draws a background box and then the messages on top.
    /// </summary>
    public void Draw(
        BitmapDisplay.BitmapDisplayPanel bitmapDisplay,
        Graphics graphics,
        DrawingSpec panelDrawingSpec,
        Func<TMessageType?, DrawingSpec> getDrawingSpecForMessageType)
    {
        Guard.ThrowIfNull(bitmapDisplay);
        Guard.ThrowIfNull(graphics);
        Guard.ThrowIfNull(panelDrawingSpec);
        Guard.ThrowIfNull(getDrawingSpecForMessageType);

        if (_messageLines.Count == 0)
        {
            return;
        }

        const int border = 15;
        var maxWidthPx = bitmapDisplay.Width - (2 * border);
        maxWidthPx = Math.Max(maxWidthPx, 100);

        var individualMessageHeights = new List<int>();
        float heightOfAllTextLines = 0;
        float textLineWidth = 0;

        foreach (MessageLine messageLine in _messageLines)
        {
            var drawingSpecForMessage = getDrawingSpecForMessageType(messageLine.Type);
            var font = DrawingToolsPool.GetFont(drawingSpecForMessage.Font);
            SizeF messageSize = graphics.MeasureString(messageLine.Text, font, width: maxWidthPx);
            textLineWidth = Math.Max(textLineWidth, messageSize.Width);
            individualMessageHeights.Add((int)messageSize.Height);
            heightOfAllTextLines += messageSize.Height;
        }

        textLineWidth = Math.Min(maxWidthPx, textLineWidth);

        Rectangle backgroundBox = new Rectangle(
            x: border,
            y: border,
            width: (int)textLineWidth,
            height: (int)heightOfAllTextLines);

        backgroundBox.Inflate(6, 6);

        using var roundedBorderHelper = new RoundedBorderHelper();

        roundedBorderHelper.CornerRadius = 9;
        roundedBorderHelper.Corners = RoundedPanelCorners.All;
        roundedBorderHelper.Rect = backgroundBox;


        var panelBrush = DrawingToolsPool.GetBrush(panelDrawingSpec.Fill);
        var panelPen = DrawingToolsPool.GetPen(panelDrawingSpec.Lines);

        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        if (roundedBorderHelper.BorderGraphicsPath != null)
        {
            graphics.FillPath(panelBrush, roundedBorderHelper.BorderGraphicsPath);
            graphics.DrawPath(panelPen, roundedBorderHelper.BorderGraphicsPath);
        }

        float nextMessageLineY = border;
        for (int index = 0; index < _messageLines.Count; index++)
        {
            var messageLine = _messageLines[index];
            var messageDrawingSpec = getDrawingSpecForMessageType(messageLine.Type);
            var font = DrawingToolsPool.GetFont(messageDrawingSpec.Font);
            var brush = DrawingToolsPool.GetBrush(messageDrawingSpec.Fill);

            var layoutRect = new RectangleF(
                x: border,
                y: nextMessageLineY,
                width: textLineWidth,
                height: individualMessageHeights[index]);

            graphics.DrawString(
                messageLine.Text,
                font,
                brush,
                layoutRect);

            nextMessageLineY += individualMessageHeights[index];
        }
    }
}
