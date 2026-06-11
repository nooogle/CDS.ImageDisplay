using System.Drawing;
using BenchmarkDotNet.Attributes;
using CDS.ImageDisplay.WinForms.Overlays;

namespace BenchmarkTests.DrawBenchmarks.ResourcePoolBenchmarks;

/// <summary>
/// Benchmarks for comparing direct <see cref="SolidBrush"/> creation/disposal
/// against retrieval from <see cref="DrawingToolsPool"/>.
/// </summary>
[MemoryDiagnoser]
internal sealed class BrushBenchmark
{
    private readonly BrushSpec _brushSpec = new() { Color = Color.Aqua };

    /// <summary>
    /// Baseline: creates a new <see cref="SolidBrush"/> and immediately disposes it.
    /// </summary>
    [Benchmark(Baseline = true)]
    public void CreateAndDisposeBrush()
    {
        using var brush = new SolidBrush(_brushSpec.Color);
    }

    /// <summary>
    /// Retrieves a pooled <see cref="Brush"/> from <see cref="DrawingToolsPool"/>.
    /// </summary>
    [Benchmark]
    public void AccessBrushFromResourcePool()
    {
        Brush brush = DrawingToolsPool.GetBrush(_brushSpec);
    }
}
