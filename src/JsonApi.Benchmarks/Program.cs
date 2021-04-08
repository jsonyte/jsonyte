using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using JsonApi.Tests.Performance;

namespace JsonApi.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            var config = new DebugBuildConfig();
#else
            var config = DefaultConfig.Instance;
#endif

            BenchmarkRunner.Run<SerializeDeserializeBenchmarks>(config);
        }
    }
}
