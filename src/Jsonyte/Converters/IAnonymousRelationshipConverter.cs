using System.Text.Json;
using Jsonyte.Serialization;

namespace Jsonyte.Converters
{
    internal interface IAnonymousRelationshipConverter
    {
        void Write(Utf8JsonWriter writer, ref TrackedResources tracked, object value);
    }
}
