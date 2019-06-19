using Newtonsoft.Json.Serialization;

namespace Jsonapi.Resolvers
{
    public class JsonApiReferenceResolver : IReferenceResolver
    {
        public object ResolveReference(object context, string reference)
        {
            throw new System.NotImplementedException();
        }

        public string GetReference(object context, object value)
        {
            throw new System.NotImplementedException();
        }

        public bool IsReferenced(object context, object value)
        {
            throw new System.NotImplementedException();
        }

        public void AddReference(object context, string reference, object value)
        {
            throw new System.NotImplementedException();
        }
    }
}
