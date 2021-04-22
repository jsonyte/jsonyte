using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Jsonyte.Serialization;
using Jsonyte.Serialization.Contracts;
using Jsonyte.Serialization.Metadata;
using Jsonyte.Validation;

namespace Jsonyte.Converters.Objects
{
    internal abstract class JsonApiDocumentConverter<T> : JsonConverter<T>
        where T : new()
    {
        private WrappedJsonConverter<AnonymousResource>? containerConverter;

        private WrappedJsonConverter<AnonymousResourceCollection>? containerCollectionConverter;

        protected abstract void ReadData(ref Utf8JsonReader reader, ref TrackedResources tracked, T document, JsonSerializerOptions options);

        protected abstract void ReadErrors(ref Utf8JsonReader reader, ref TrackedResources tracked, T document, JsonSerializerOptions options);

        protected abstract void ReadJsonApi(ref Utf8JsonReader reader, ref TrackedResources tracked, T document, JsonSerializerOptions options);

        protected abstract void ReadMeta(ref Utf8JsonReader reader, ref TrackedResources tracked, T document, JsonSerializerOptions options);

        protected abstract void ReadLinks(ref Utf8JsonReader reader, ref TrackedResources tracked, T document, JsonSerializerOptions options);

        protected abstract void ReadIncluded(ref Utf8JsonReader reader, ref TrackedResources tracked, T document, JsonSerializerOptions options);

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var document = new T();

            var state = reader.ReadDocument();
            var tracked = new TrackedResources();

            Utf8JsonReader savedReader = default;
            var includedReadFirst = false;

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == DocumentFlags.Data)
                {
                    ReadData(ref reader, ref tracked, document, options);
                }
                else if (name == DocumentFlags.Errors)
                {
                    ReadErrors(ref reader, ref tracked, document, options);
                }
                else if (name == DocumentFlags.Jsonapi)
                {
                    ReadJsonApi(ref reader, ref tracked, document, options);
                }
                else if (name == DocumentFlags.Meta)
                {
                    ReadMeta(ref reader, ref tracked, document, options);
                }
                else if (name == DocumentFlags.Links)
                {
                    ReadLinks(ref reader, ref tracked, document, options);
                }
                else if (name == DocumentFlags.Included)
                {
                    if (state.HasFlag(DocumentFlags.Data))
                    {
                        ReadIncluded(ref reader, ref tracked, document, options);
                    }
                    else
                    {
                        includedReadFirst = true;
                        savedReader = reader;

                        reader.Skip();
                    }
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            if (includedReadFirst)
            {
                ReadIncluded(ref savedReader, ref tracked, document, options);
            }

            state.Validate();

            return state.IsEmpty()
                ? default
                : document;
        }

        protected TConverter? ReadWrapped<TConverter>(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonSerializerOptions options)
        {
            if (options.GetConverter(typeof(TConverter)) is not WrappedJsonConverter<TConverter> converter)
            {
                throw new JsonApiException($"Could not find converter for type '{typeof(TConverter)}'");
            }

            return converter.ReadWrapped(ref reader, ref tracked, typeof(TConverter), default, options);
        }

        protected abstract void WriteData(Utf8JsonWriter writer, ref TrackedResources tracked, T value, JsonSerializerOptions options);

        protected abstract void WriteErrors(Utf8JsonWriter writer, ref TrackedResources tracked, T value, JsonSerializerOptions options);

        protected abstract void WriteLinks(Utf8JsonWriter writer, ref TrackedResources tracked, T value, JsonSerializerOptions options);

        protected abstract void WriteMeta(Utf8JsonWriter writer, ref TrackedResources tracked, T value, JsonSerializerOptions options);

        protected abstract void WriteJsonApi(Utf8JsonWriter writer, ref TrackedResources tracked, T value, JsonSerializerOptions options);

        protected abstract void WriteIncluded(Utf8JsonWriter writer, ref TrackedResources tracked, T value, JsonSerializerOptions options);

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var tracked = new TrackedResources();

            ValidateDocument(value);

            writer.WriteStartObject();

            WriteData(writer, ref tracked, value, options);
            WriteErrors(writer, ref tracked, value, options);
            WriteLinks(writer, ref tracked, value, options);
            WriteMeta(writer, ref tracked, value, options);
            WriteJsonApi(writer, ref tracked, value, options);
            WriteIncluded(writer, ref tracked, value, options);

            writer.WriteEndObject();
        }

        protected void WriteIfNotNull<TValue>(Utf8JsonWriter writer, JsonEncodedText name, TValue value, JsonSerializerOptions options)
        {
            if (value != null)
            {
                writer.WritePropertyName(name);
                JsonSerializer.Serialize(writer, value, options);
            }
        }

        protected void WriteWrapped<TElement>(Utf8JsonWriter writer, ref TrackedResources tracked, TElement value, JsonSerializerOptions options)
        {
            var type = value!.GetType();
            var converter = options.GetConverter(type);

            if (converter is WrappedJsonConverter<TElement> genericConverter)
            {
                genericConverter.WriteWrapped(writer, ref tracked, value, options);

                return;
            }

            var category = type.GetTypeCategory();

            if (category == JsonTypeCategory.Object)
            {
                var container = new AnonymousResource(value);

                GetContainerConverter(options).WriteWrapped(writer, ref tracked, container, options);
            }
            else
            {
                var container = new AnonymousResourceCollection(value);

                GetContainerCollectionConverter(options).WriteWrapped(writer, ref tracked, container, options);
            }
        }

        [AssertionMethod]
        protected abstract void ValidateDocument(T document);

        private WrappedJsonConverter<AnonymousResource> GetContainerConverter(JsonSerializerOptions options)
        {
            return containerConverter ??= options.GetWrappedConverter<AnonymousResource>();
        }

        private WrappedJsonConverter<AnonymousResourceCollection> GetContainerCollectionConverter(JsonSerializerOptions options)
        {
            return containerCollectionConverter ??= options.GetWrappedConverter<AnonymousResourceCollection>();
        }
    }
}
