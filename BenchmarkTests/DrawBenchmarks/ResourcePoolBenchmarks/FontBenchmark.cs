using System.Drawing;
using BenchmarkDotNet.Attributes;

namespace BenchmarkTests.DrawBenchmarks.ResourcePoolBenchmarks;

[MemoryDiagnoser]
public class FontBenchmark
{
    private readonly CDS.ImageDisplay.Overlays.FontSpec fontSpec = new()
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
        Font font = CDS.ImageDisplay.Overlays.DrawingToolsPool.GetFont(fontSpec);
    }
}
