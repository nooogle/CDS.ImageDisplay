using BenchmarkDotNet.Running;
using CDS.Imaging.Utils;

namespace BenchmarkTests;


class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(SystemInfo.Get());

        Console.WriteLine(
            "\nNOTE: the resource pool throws an exception if it's used on a non-UI thread. Disable the" +
            "check (temporarily!) to test it from this program.\n");
        
        Console.Write("Press any key to start > ");
        Console.ReadKey();
        Console.WriteLine();

        // Run all benchmarks in the assembly
        var switcher = new BenchmarkSwitcher(
        [
            typeof(DrawBenchmarks.ResourcePoolBenchmarks.PenBenchmark),
            typeof(DrawBenchmarks.ResourcePoolBenchmarks.BrushBenchmark),
            typeof(DrawBenchmarks.ResourcePoolBenchmarks.FontBenchmark)
        ]);
        switcher.Run(args);
    }
}
