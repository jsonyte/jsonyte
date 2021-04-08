using System;
using System.Diagnostics;
using System.Linq;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;
using Xunit;
using Xunit.Abstractions;

namespace JsonApi.Tests.Performance
{
    public class BenchmarkTests
    {
        private readonly ITestOutputHelper testOutput;

        public BenchmarkTests(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        [Fact]
        public void RunBenchmarks()
        {
            var logger = new XunitBenchmarkLogger(testOutput);
            var config = CreateConfig();

            var summary = BenchmarkRunner.Run<SerializeDeserializeBenchmarks>(config);

            if (summary.ValidationErrors.Any(x => x.IsCritical))
            {
                throw new InvalidOperationException("Benchmarks failed to run due to validation errors");
            }

            MarkdownExporter.Console.ExportToLog(summary, logger);
        }

        private IConfig CreateConfig()
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                return new DebugInProcessConfig();
            }

            return new DebugBuildConfig();
#else
            return DefaultConfig.Instance;
#endif
        }
    }
}
