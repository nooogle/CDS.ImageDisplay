using System.Drawing;
using BenchmarkDotNet.Attributes;

namespace BenchmarkTests.DrawBenchmarks.ResourcePoolBenchmarks;

[MemoryDiagnoser]
internal sealed class PenBenchmark : IDisposable
{
    private readonly Pen _pen = new(Color.White);

    public void Dispose()
    {
        _pen.Dispose();
    }

    private static readonly CDS.ImageDisplay.WinForms   .Overlays.PenSpec s_lineSpec1 = new()
    {
        Color = Color.RebeccaPurple,
        StartCap = System.Drawing.Drawing2D.LineCap.Round,
        EndCap = System.Drawing.Drawing2D.LineCap.Triangle,
        DashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
        Width = 2,
    };

    private static readonly CDS.ImageDisplay.WinForms.Overlays.PenSpec s_lineSpec2 = new()
    {
        Color = Color.Wheat,
        StartCap = System.Drawing.Drawing2D.LineCap.DiamondAnchor,
        EndCap = System.Drawing.Drawing2D.LineCap.SquareAnchor,
        DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot,
        Width = 4,
    };


    [Benchmark(Baseline = true)]
    public static void CreateAndDisposePen()
    {
        var pen = new Pen(color: s_lineSpec1.Color, width: s_lineSpec1.Width)
        {
            DashStyle = s_lineSpec1.DashStyle,
            StartCap = s_lineSpec1.StartCap,
            EndCap = s_lineSpec1.EndCap
        };
        pen.Dispose();

        pen = new Pen(color: s_lineSpec2.Color, width: s_lineSpec2.Width)
        {
            Width = s_lineSpec2.Width,
            DashStyle = s_lineSpec2.DashStyle,
            StartCap = s_lineSpec2.StartCap,
            EndCap = s_lineSpec2.EndCap
        };
        pen.Dispose();
    }


    [Benchmark]
    public static void AccessPenFromResourcePool()
    {
        var pen = CDS.ImageDisplay.WinForms.Overlays.DrawingToolsPool.GetPen(s_lineSpec1);
        pen = CDS.ImageDisplay.WinForms.Overlays.DrawingToolsPool.GetPen(s_lineSpec1);
    }

    [Benchmark]
    public void ChangeExistingPen()
    {
        _pen.Color = s_lineSpec1.Color;
        _pen.Width = s_lineSpec1.Width;
        _pen.DashStyle = s_lineSpec1.DashStyle;
        _pen.StartCap = s_lineSpec1.StartCap;
        _pen.EndCap = s_lineSpec1.EndCap;

        _pen.Color = s_lineSpec2.Color;
        _pen.Width = s_lineSpec2.Width;
        _pen.DashStyle = s_lineSpec2.DashStyle;
        _pen.StartCap = s_lineSpec2.StartCap;
        _pen.EndCap = s_lineSpec2.EndCap;

    }
}
