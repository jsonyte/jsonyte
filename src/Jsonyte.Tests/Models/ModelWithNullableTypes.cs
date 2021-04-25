using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ModelWithNullableTypes
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("byteValue")]
        public byte? ByteValue { get; set; }

        [JsonPropertyName("sbyteValue")]
        public sbyte? SbyteValue { get; set; }

        [JsonPropertyName("decimalValue")]
        public decimal? DecimalValue { get; set; }

        [JsonPropertyName("shortValue")]
        public short? ShortValue { get; set; }

        [JsonPropertyName("ushortValue")]
        public ushort? UshortValue { get; set; }

        [JsonPropertyName("intValue")]
        public int? IntValue { get; set; }

        [JsonPropertyName("uintValue")]
        public uint? UintValue { get; set; }

        [JsonPropertyName("longValue")]
        public long? LongValue { get; set; }

        [JsonPropertyName("ulongValue")]
        public ulong? UlongValue { get; set; }

        [JsonPropertyName("floatValue")]
        public float? FloatValue { get; set; }

        [JsonPropertyName("doubleValue")]
        public double? DoubleValue { get; set; }

        [JsonPropertyName("objectValue")]
        public object ObjectValue { get; set; }
    }
}
