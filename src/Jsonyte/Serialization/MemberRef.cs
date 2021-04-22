using Jsonyte.Serialization.Metadata;

namespace Jsonyte.Serialization
{
    internal readonly struct MemberRef
    {
        public readonly ulong Key;

        public readonly IJsonMemberInfo Member;

        public readonly byte[] Name;

        public MemberRef(ulong key, IJsonMemberInfo member, byte[] name)
        {
            Key = key;
            Member = member;
            Name = name;
        }
    }
}
