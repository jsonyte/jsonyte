using System;
using System.Collections.Generic;
using System.Text;

namespace Jsonyte.Serialization.Attributes
{

    /// <summary>
    /// Specifies how a relationship should be serialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SerializeAsAttribute : Attribute
    {
        /// <summary>
        /// Indicates the type of serialization for the relationship.
        /// </summary>
        public RelationshipSerializationType Type { get; }

        /// <summary>
        /// Constructor to initialize the serialization type.
        /// </summary>
        /// <param name="type"></param>
        public SerializeAsAttribute(RelationshipSerializationType type = RelationshipSerializationType.Included)
        {
            Type = type;
        }
    }
}
