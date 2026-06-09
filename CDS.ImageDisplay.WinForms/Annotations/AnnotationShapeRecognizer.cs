using System;
using System.Collections.Generic;
using System.Linq;

namespace CDS.ImageDisplay.Annotations;

/// <summary>
/// Ranks <see cref="IAnnotationShapeDescriptor"/> instances against a freehand gesture path
/// and returns them sorted by descending confidence.
/// </summary>
public static class AnnotationShapeRecognizer
{
    /// <summary>
    /// Minimum <see cref="IAnnotationShapeDescriptor.FitScore"/> for the Auto pick to choose a
    /// specific shape over the polygon fallback.
    /// </summary>
    public const float ConfidenceThreshold = 0.5f;

    /// <summary>
    /// Scores each descriptor against <paramref name="path"/> and returns the results
    /// sorted by confidence descending.
    /// </summary>
    /// <param name="path">The freehand gesture to score against.</param>
    /// <param name="descriptors">The descriptors to rank.</param>
    public static IReadOnlyList<(IAnnotationShapeDescriptor Descriptor, float Confidence)> Rank(
        FreehandPath path,
        IEnumerable<IAnnotationShapeDescriptor> descriptors)
    {
        Guard.ThrowIfNull(path, nameof(path));
        Guard.ThrowIfNull(descriptors, nameof(descriptors));

        return descriptors
            .Select(d => (Descriptor: d, Confidence: d.FitScore(path)))
            .OrderByDescending(x => x.Confidence)
            .ToList()
            .AsReadOnly();
    }
}
