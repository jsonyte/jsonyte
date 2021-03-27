using System;
using System.Buffers;
using System.Text.Json;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiResourceConstructorConverter<T> : JsonApiConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var resource = default(T);

            var state = reader.ReadDocument();

            while (reader.IsObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == JsonApiMembers.Data)
                {
                    resource = ReadWrapped(ref reader, typeToConvert, options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            state.Validate();

            return resource;
        }

        public override T? ReadWrapped(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            reader.ReadObject("resource");

            var info = options.GetClassInfo(typeToConvert);

            ValidateResource(info);

            var state = new JsonApiResourceState();

            var parameters = ArrayPool<object>.Shared.Rent(info.ParameterCount);
            
            while (reader.IsObject())
            {
                var name = reader.ReadMember("resource object");

                state.AddFlag(name);

                var parameter = info.GetParameter(name);

                if (name == JsonApiMembers.Attributes)
                {
                    reader.ReadObject("resource attributes");

                    while (reader.IsObject())
                    {
                        var attributeName = reader.ReadMember("resource object");
                        var attributeParameter = info.GetParameter(attributeName);

                        var value = attributeParameter?.Read(ref reader);

                        if (value != null)
                        {
                            parameters[attributeParameter!.Position] = value;
                        }

                        reader.Read();
                    }
                }
                else
                {
                    var value = parameter?.Read(ref reader);

                    if (value != null)
                    {
                        parameters[parameter!.Position] = value;
                    }
                }

                reader.Read();
            }

            state.Validate();

            var resource = info.CreatorWithArguments(parameters);

            if (resource == null)
            {
                return default;
            }

            ArrayPool<object>.Shared.Return(parameters, true);

            return (T) resource;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private void ValidateResource(JsonTypeInfo info)
        {
            var idProperty = info.GetMember(JsonApiMembers.Id);

            if (!string.IsNullOrEmpty(idProperty.Name) && idProperty.MemberType != typeof(string))
            {
                throw new JsonApiException("JSON:API resource id must be a string");
            }

            var typeProperty = info.GetMember(JsonApiMembers.Type);

            if (string.IsNullOrEmpty(typeProperty.Name))
            {
                throw new JsonApiException("JSON:API resource must have a 'type' member");
            }

            if (typeProperty.MemberType != typeof(string))
            {
                throw new JsonApiException("JSON:API resource type must be a string");
            }
        }
    }
}
