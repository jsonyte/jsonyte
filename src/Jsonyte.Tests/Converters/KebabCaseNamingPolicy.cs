using System.Text.Json;
using Humanizer;

namespace Jsonyte.Tests.Converters
{
    public class KebabCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.Kebaberize();
        }
    }
}
