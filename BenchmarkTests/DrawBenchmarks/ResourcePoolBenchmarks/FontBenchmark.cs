using BenchmarkDotNet.Attributes;
using System.Drawing;

namespace BenchmarkTests.DrawBenchmarks.ResourcePoolBenchmarks;

[MemoryDiagnoser]
public class FontBenchmark
{
    private CDS.ImageDisplay.Overlays.FontSpec fontSpec = new CDS.ImageDisplay.Overlays.FontSpec()
    {
        FontName = "Arial",
        FontSize = 12,
    };


    [Benchmark(Baseline = true)]
    public void CreateAndDisposeFont()
    {
        var font = new Font(fontSpec.FontName, fontSpec.FontSize);
        font.Dispose();
    }


    [Benchmark]
    public void AccessFontFromResourcePool()
    {
        var font = CDS.ImageDisplay.Overlays.DrawingToolsPool.GetFont(fontSpec);
    }
}
