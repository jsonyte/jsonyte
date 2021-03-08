namespace JsonApi
{
    internal ref struct JsonApiDocumentState
    {
        private JsonApiDocumentFlags flags;

        public void AddFlag(string? member)
        {
            flags = flags.AddFlag(member);
        }

        public void Validate()
        {
            flags.Validate();
        }

        public JsonApiDocumentFlags ToFlags()
        {
            return flags;
        }

        public bool HasFlag(JsonApiDocumentFlags flag)
        {
            return flags.HasFlag(flag);
        }
    }
}
