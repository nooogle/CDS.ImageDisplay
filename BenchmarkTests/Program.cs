using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Drawing;

namespace BenchmarkTests;

public class PenBenchmark
{
    private CDS.Imaging.WinForms.Draw.LineSpec lineSpec = new CDS.Imaging.WinForms.Draw.LineSpec()
    {
        Color = Color.Black,
        Width = 1,
        DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot,
        EndCap = System.Drawing.Drawing2D.LineCap.Round,
        StartCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor,
    };

    [Benchmark]
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
        var pen = CDS.Imaging.WinForms.Draw.RenderingToolsPool.GetPen(lineSpec);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(CDS.Imaging.WinForms.SystemInfo.Get());
        Console.Write("Press any key to start > ");
        Console.ReadKey();
        Console.WriteLine();

        var summary = BenchmarkRunner.Run<PenBenchmark>();
    }
}
