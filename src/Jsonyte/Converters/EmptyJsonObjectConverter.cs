using System.Text.Json;
using Jsonyte.Serialization;
using Jsonyte.Validation;

namespace Jsonyte.Converters
{
    internal class EmptyJsonObjectConverter : JsonObjectConverter
    {
        public static EmptyJsonObjectConverter Default { get; } = new();

        public override void Read(ref Utf8JsonReader reader, ref TrackedResources tracked, object existingValue, JsonSerializerOptions options)
        {
            reader.ReadResource();

            var state = new ResourceState();

            while (reader.IsInObject())
            {
                reader.ReadMember(ref state);

                reader.Skip();
                reader.Read();
            }

            state.Validate();
        }

        public override void Write(Utf8JsonWriter writer, ref TrackedResources tracked, object value, JsonSerializerOptions options)
        {
        }
    }
}
