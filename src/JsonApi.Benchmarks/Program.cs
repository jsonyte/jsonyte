using BenchmarkDotNet.Running;
using JsonApi.Tests.Performance;

namespace JsonApi.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(BenchmarkTests));
        }
    }
}
