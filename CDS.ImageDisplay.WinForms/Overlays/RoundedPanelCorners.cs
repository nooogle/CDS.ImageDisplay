using System;

namespace CDS.ImageDisplay.WinForms.Overlays;


/// <summary>
/// Describes the corners that can be make rounded. The values can be
/// OR'd together.
/// </summary>
[Flags]
public enum RoundedPanelCorners
{
    /// <summary>No corners are rounded.</summary>
    None = 0x00,


    /// <summary>Top-left corner.</summary>
    TopLeft = 0x01,


    /// <summary>Top-right corner.</summary>
    TopRight = 0x02,


    /// <summary>Bottom-right corner.</summary>
    BottomRight = 0x04,


    /// <summary>Bottom-left corner.</summary>
    BottomLeft = 0x08,


    /// <summary>All corners are rounded.</summary>
    All = TopLeft | TopRight | BottomLeft | BottomRight,
}
