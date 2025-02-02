using BenchmarkDotNet.Attributes;
using System.Drawing;

namespace BenchmarkTests.DrawBenchmarks.ResourcePoolBenchmarks;

[MemoryDiagnoser]
public class PenBenchmark
{
    private Pen pen = new Pen(Color.White);

    private static readonly CDS.Imaging.Draw.PenSpec lineSpec1 = new CDS.Imaging.Draw.PenSpec()
    {
        Color = Color.RebeccaPurple,
        StartCap = System.Drawing.Drawing2D.LineCap.Round,
        EndCap = System.Drawing.Drawing2D.LineCap.Triangle,
        DashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
        Width = 2,
    };

    private static readonly CDS.Imaging.Draw.PenSpec lineSpec2 = new CDS.Imaging.Draw.PenSpec()
    {
        Color = Color.Wheat,
        StartCap = System.Drawing.Drawing2D.LineCap.DiamondAnchor,
        EndCap = System.Drawing.Drawing2D.LineCap.SquareAnchor,
        DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot,
        Width = 4,
    };


    [Benchmark(Baseline = true)]
    public void CreateAndDisposePen()
    {
        var pen = new Pen(color: lineSpec1.Color, width: lineSpec1.Width);
        pen.DashStyle = lineSpec1.DashStyle;
        pen.StartCap = lineSpec1.StartCap;
        pen.EndCap = lineSpec1.EndCap;
        pen.Dispose();

        pen = new Pen(color: lineSpec2.Color, width: lineSpec2.Width);
        pen.Width = lineSpec2.Width;
        pen.DashStyle = lineSpec2.DashStyle;
        pen.StartCap = lineSpec2.StartCap;
        pen.EndCap = lineSpec2.EndCap;
        pen.Dispose();
    }


    [Benchmark]
    public void AccessPenFromResourcePool()
    {
        var pen = CDS.Imaging.Draw.RenderingToolsPool.GetPen(lineSpec1);
        pen = CDS.Imaging.Draw.RenderingToolsPool.GetPen(lineSpec1);
    }

    [Benchmark]
    public void ChangeExistingPen()
    {
        pen.Color = lineSpec1.Color;
        pen.Width = lineSpec1.Width;
        pen.DashStyle = lineSpec1.DashStyle;
        pen.StartCap = lineSpec1.StartCap;
        pen.EndCap = lineSpec1.EndCap;

        pen.Color = lineSpec2.Color;
        pen.Width = lineSpec2.Width;
        pen.DashStyle = lineSpec2.DashStyle;
        pen.StartCap = lineSpec2.StartCap;
        pen.EndCap = lineSpec2.EndCap;

    }
}
