using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Drawing;

namespace BenchmarkTests;

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


[MemoryDiagnoser]
public class BrushBenchmark
{
    private CDS.Imaging.Draw.BrushSpec brushSpec = new CDS.Imaging.Draw.BrushSpec()
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
        var brush = CDS.Imaging.Draw.RenderingToolsPool.GetBrush(brushSpec);
    }
}


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



class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(CDS.Imaging.SystemInfo.Get());
        Console.Write("Press any key to start > ");
        Console.ReadKey();
        Console.WriteLine();

        // Run all benchmarks in the assembly
        var switcher = new BenchmarkSwitcher(
        [
            typeof(PenBenchmark),
            typeof(BrushBenchmark),
            typeof(FontBenchmark)
        ]);
        switcher.Run(args);
    }
}
