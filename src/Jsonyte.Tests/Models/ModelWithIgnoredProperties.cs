using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ModelWithIgnoredProperties
    {
        public string Type { get; set; } = "type";

        [JsonIgnore]
        public string TitleIgnored { get; set; } = "title";

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public string TitleNeverIgnored { get; set; } = "title";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string TitleIgnoreDefault { get; set; } = "title";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string TitleIgnoreNull { get; set; } = "title";

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string TitleIgnoreAlways { get; set; } = "title";

        [JsonIgnore]
        public int CountIgnored { get; set; } = 5;

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public int CountNeverIgnored { get; set; } = 5;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int CountIgnoreDefault { get; set; } = 5;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int CountIgnoreNull { get; set; } = 5;

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public int CountIgnoreAlways { get; set; } = 5;

        [JsonIgnore]
        public int? NullableCountIgnored { get; set; } = 5;

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public int? NullableCountNeverIgnored { get; set; } = 5;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? NullableCountIgnoreDefault { get; set; } = 5;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? NullableCountIgnoreNull { get; set; } = 5;

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public int? NullableCountIgnoreAlways { get; set; } = 5;
    }
}
