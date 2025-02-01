using BenchmarkDotNet.Attributes;
using System.Drawing;

namespace BenchmarkTests.DrawBenchmarks.ResourcePoolBenchmarks;

[MemoryDiagnoser]
public class PenBenchmark
{
    private CDS.Imaging.Draw.LineSpec lineSpec = new CDS.Imaging.Draw.LineSpec()
    {
        Color = Color.Black,
        Width = 1,
        DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot,
        EndCap = System.Drawing.Drawing2D.LineCap.Round,
        StartCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor,
    };


    [Benchmark(Baseline = true)]
    public void CreateAndDisposePen()
    {
        var pen = new Pen(color: lineSpec.Color, width: lineSpec.Width);
        pen.DashStyle = lineSpec.DashStyle;
        pen.EndCap = lineSpec.EndCap;
        pen.StartCap = lineSpec.StartCap;

        pen.Dispose();
    }


    [Benchmark]
    public void AccessPenFromResourcePool()
    {
        var pen = CDS.Imaging.Draw.RenderingToolsPool.GetPen(lineSpec);
    }
}
