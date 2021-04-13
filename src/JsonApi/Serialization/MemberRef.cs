namespace JsonApi.Serialization
{
    internal readonly struct MemberRef
    {
        public MemberRef(ulong key, IJsonMemberInfo member, byte[] name)
        {
            Key = key;
            Member = member;
            Name = name;
        }

        public readonly ulong Key;

        public readonly IJsonMemberInfo Member;

        public readonly byte[] Name;
    }
}
