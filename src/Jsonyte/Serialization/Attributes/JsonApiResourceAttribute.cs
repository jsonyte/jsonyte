using System;
namespace Jsonyte.Serialization.Attributes
{
    /// <summary>
    /// This attribute allows you to declare a class as a JSON:API Resource Object and specify the type field. This takes the place of a separate Type field on a class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class JsonApiResourceAttribute : Attribute
    {
        /// <summary>
        /// Constructor to declare a Resource Object
        /// </summary>
        /// <param name="type">The type associated with the Resource Object</param>
        public JsonApiResourceAttribute(string type)
        {
            this.Type = type;
        }

        /// <summary>
        /// The Type associated with the Resource Object. This will always get serialized with the Id attribute
        /// </summary>
        public string Type { get; }
    }
}
