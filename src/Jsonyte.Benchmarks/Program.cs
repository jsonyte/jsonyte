using System.Diagnostics;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Jsonyte.Tests.Performance;

namespace Jsonyte.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            var config = Debugger.IsAttached
                ? (IConfig) new DebugInProcessConfig()
                : new DebugBuildConfig();
#else
            var config = DefaultConfig.Instance;
#endif

            BenchmarkRunner.Run<MicroPerformanceBenchmarks>(config);
        }
    }
}
