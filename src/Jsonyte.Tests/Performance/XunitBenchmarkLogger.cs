using System.Text;
using BenchmarkDotNet.Loggers;
using Xunit.Abstractions;

namespace Jsonyte.Tests.Performance
{
    public class XunitBenchmarkLogger : ILogger
    {
        private readonly ITestOutputHelper output;

        private readonly StringBuilder buffer = new();
        
        public XunitBenchmarkLogger(ITestOutputHelper output)
        {
            this.output = output;
        }

        public string Id { get; } = "xunit";

        public int Priority { get; } = 1000;

        public void Write(LogKind logKind, string text)
        {
            buffer.Append(text);
        }

        public void WriteLine()
        {
            if (buffer.Length > 0)
            {
                output.WriteLine(buffer.ToString());

                buffer.Clear();
            }
            else
            {
                output.WriteLine(string.Empty);
            }
        }

        public void WriteLine(LogKind logKind, string text)
        {
            if (buffer.Length > 0)
            {
                buffer.Append(text);
                output.WriteLine(buffer.ToString());

                buffer.Clear();
            }
            else
            {
                output.WriteLine(text);
            }
        }

        public void Flush()
        {
        }
    }
}
