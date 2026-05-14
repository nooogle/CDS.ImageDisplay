using System.Drawing;
using BenchmarkDotNet.Attributes;

namespace BenchmarkTests.DrawBenchmarks.ResourcePoolBenchmarks;

[MemoryDiagnoser]
public class BrushBenchmark
{
    private readonly CDS.ImageDisplay.Overlays.BrushSpec brushSpec = new()
    {
        Color = Color.Aqua,
    };


    [Benchmark(Baseline = true)]
    public void CreateAndDisposeBrush()
    {
        var brush = new SolidBrush(brushSpec.Color);
        brush.Dispose();
    }


    [Benchmark]
    public void AccessBrushFromResourcePool()
    {
        Brush brush = CDS.ImageDisplay.Overlays.DrawingToolsPool.GetBrush(brushSpec);
    }
}
