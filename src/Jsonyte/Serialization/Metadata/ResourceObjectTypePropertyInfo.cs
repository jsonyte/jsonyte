using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Serialization.Attributes;

namespace Jsonyte.Serialization.Metadata
{
    /// <summary>
    /// This contains the information to serialize the Type attribute when it is declared using the ResourceObjectAttribute
    /// </summary>
    internal sealed class ResourceObjectAttributeMemberInfo: JsonMemberInfo<string>
    {
        private ResourceObjectAttribute resourceObject;

        public ResourceObjectAttributeMemberInfo(ResourceObjectAttribute resourceObject, JsonSerializerOptions options)
            : base(typeof(ResourceObjectAttribute).GetProperty(nameof(resourceObject.Type))!, typeof(string), JsonIgnoreCondition.Never, options.GetConverter(typeof(string)), options)
        {
            this.resourceObject = resourceObject;
        }

        public override Func<object, string>? Get => (object theObject) => this.resourceObject.Type;

        public override Action<object, string>? Set => null; //ResourceObjectAttribute is already set at compile time so we don't need to set anything

        public override bool Ignored => false;
    }
}

