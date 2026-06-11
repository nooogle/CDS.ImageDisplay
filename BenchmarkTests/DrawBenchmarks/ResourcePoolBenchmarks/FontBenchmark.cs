using System.Drawing;
using BenchmarkDotNet.Attributes;
using CDS.ImageDisplay.WinForms.Overlays;

namespace BenchmarkTests.DrawBenchmarks.ResourcePoolBenchmarks;

/// <summary>
/// Benchmarks for comparing direct <see cref="Font"/> creation/disposal
/// against retrieval from <see cref="DrawingToolsPool"/>.
/// </summary>
[MemoryDiagnoser]
internal sealed class FontBenchmark
{
    private readonly FontSpec _fontSpec = new()
    {
        FontName = "Arial",
        FontSize = 12,
    };

    /// <summary>
    /// Baseline: creates a new <see cref="Font"/> and immediately disposes it.
    /// </summary>
    [Benchmark(Baseline = true)]
    public void CreateAndDisposeFont()
    {
        using var font = new Font(_fontSpec.FontName, _fontSpec.FontSize);
    }

    /// <summary>
    /// Retrieves a pooled <see cref="Font"/> from <see cref="DrawingToolsPool"/>.
    /// </summary>
    [Benchmark]
    public void AccessFontFromResourcePool()
    {
        Font font = DrawingToolsPool.GetFont(_fontSpec);
    }
}
