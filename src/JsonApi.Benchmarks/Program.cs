using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using JsonApi.Tests.Performance;

namespace JsonApi.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var tests = new BenchmarkTests(null);
            tests.Setup();

            for (var i = 0; i < 1000000; i++)
            {
                tests.SerializeSimpleObject();
            }


            //var summary = BenchmarkRunner.Run(typeof(BenchmarkTests));
        }
    }
}
