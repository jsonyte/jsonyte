namespace Jsonyte.Converters.Objects
{
    // TODO: Not working yet
#if CONSTRUCTOR_CONVERTER
    internal class JsonApiResourceObjectConstructorConverter<T> : JsonApiResourceObjectConverter<T>
    {
        public JsonApiResourceObjectConstructorConverter(JsonTypeInfo type)
            : base(type)
        {
        }

        public override T? ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, T? existingValue, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            var state = reader.ReadResource();

            var info = options.GetTypeInfo(typeToConvert);

            var parameters = ArrayPool<object>.Shared.Rent(info.ParameterCount);
            var properties = ArrayPool<(IJsonMemberInfo Member, object? Value)>.Shared.Rent(info.MemberCount);
            var propertiesUsed = 0;
            
            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == JsonApiMembers.Attributes)
                {
                    reader.ReadObject(JsonApiMemberCode.ResourceAttributes);

                    while (reader.IsInObject())
                    {
                        var attributeName = reader.ReadMember(JsonApiMemberCode.Resource);

                        ReadValue(ref reader, info, attributeName, parameters, properties, ref propertiesUsed);

                        reader.Read();
                    }
                }
                else
                {
                    ReadValue(ref reader, info, name, parameters, properties, ref propertiesUsed);
                }

                reader.Read();
            }

            state.Validate();

            var resource = info.CreatorWithArguments(parameters);

            if (resource == null)
            {
                return default;
            }

            for (var i = 0; i < propertiesUsed; i++)
            {
                var property = properties[i];

                property.Member.SetValue(resource, property.Value);
            }

            ArrayPool<object>.Shared.Return(parameters, true);
            ArrayPool<(IJsonMemberInfo Member, object? Value)>.Shared.Return(properties, true);

            return (T) resource;
        }

        private void ReadValue(
            ref Utf8JsonReader reader,
            JsonTypeInfo info,
            string? name,
            object[] parameters,
            (IJsonMemberInfo member, object? value)[] properties,
            ref int propertiesUsed)
        {
            var parameter = info.GetParameter(name);

            if (parameter != null)
            {
                var value = parameter.Read(ref reader);

                if (value != null)
                {
                    parameters[parameter!.Position] = value;
                }
            }
            else
            {
                var property = info.GetMember(name);

                var value = property.Read(ref reader);

                properties[propertiesUsed++] = (property, value);
            }
        }
    }
#endif
}
