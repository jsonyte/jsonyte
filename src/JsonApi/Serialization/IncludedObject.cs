namespace JsonApi.Serialization
{
    internal struct IncludedObject
    {
        public IncludedObject(object value, IJsonMemberInfo member)
        {
            Value = value;
            Member = member;
        }

        public object Value;

        public IJsonMemberInfo Member;
    }
}
