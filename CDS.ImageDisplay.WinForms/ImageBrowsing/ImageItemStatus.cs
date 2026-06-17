using System.Drawing;

namespace CDS.ImageDisplay.WinForms.ImageBrowsing;

/// <summary>
/// Carries the status information rendered as a per-row indicator in <see cref="ImageListPanel"/>.
/// </summary>
/// <remarks>
/// The host application is responsible for choosing colours and badge text that are meaningful
/// in its domain. The library renders only what is supplied here and attaches no semantic meaning
/// to any particular colour or text.
/// </remarks>
/// <param name="Color">The colour used to tint the list row background.</param>
/// <param name="BadgeText">
/// Optional short text prepended to the filename (e.g. <c>"OK"</c>, <c>"ERR"</c>).
/// Pass <see langword="null"/> or an empty string to suppress the badge.
/// </param>
public sealed record ImageItemStatus(Color Color, string? BadgeText = null);
