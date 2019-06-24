using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace Jsonapi.Resolvers
{
    public class JsonApiReferenceResolver : IReferenceResolver
    {
        private readonly Dictionary<string, object> references = new Dictionary<string, object>();

        public object ResolveReference(object context, string reference)
        {
            references.TryGetValue(reference, out var value);

            return value;
        }

        public string GetReference(object context, object value)
        {
            throw new NotSupportedException();
        }

        public bool IsReferenced(object context, object value)
        {
            throw new NotSupportedException();
        }

        public void AddReference(object context, string reference, object value)
        {
            references.Add(reference, value);
        }
    }
}
