using System;
using System.Collections.Generic;
using System.Text;

namespace Jsonyte.Serialization
{
    /// <summary>
    /// Defines the serialization type for relationships in JSON:API.
    /// </summary>
    public enum RelationshipSerializationType : byte
    {
        /// <summary>
        /// The relationship is serialized as a resource object with both 'id' and 'type' properties.
        /// </summary>
        IdAndType,

        /// <summary>
        /// This relationship is serialized with all properties included in the resource object.
        /// </summary>
        Included
    }
}
