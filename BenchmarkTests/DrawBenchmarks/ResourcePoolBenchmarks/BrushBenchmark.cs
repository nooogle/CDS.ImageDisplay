using BenchmarkDotNet.Attributes;
using System.Drawing;

namespace BenchmarkTests.DrawBenchmarks.ResourcePoolBenchmarks;

[MemoryDiagnoser]
public class BrushBenchmark
{
    private CDS.Imaging.Overlays.BrushSpec brushSpec = new CDS.Imaging.Overlays.BrushSpec()
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
        var brush = CDS.Imaging.Overlays.DrawingToolsPool.GetBrush(brushSpec);
    }
}
