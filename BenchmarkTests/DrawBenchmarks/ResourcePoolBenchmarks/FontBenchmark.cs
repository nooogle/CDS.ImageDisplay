using BenchmarkDotNet.Attributes;
using System.Drawing;

namespace BenchmarkTests.DrawBenchmarks.ResourcePoolBenchmarks;

[MemoryDiagnoser]
public class FontBenchmark
{
    private CDS.Imaging.Draw.FontSpec fontSpec = new CDS.Imaging.Draw.FontSpec()
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
        var font = CDS.Imaging.Draw.RenderingToolsPool.GetFont(fontSpec);
    }
}
