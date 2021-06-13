using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jsonyte
{
    /// <summary>
    /// Represents a <see href="https://jsonapi.org/">JSON:API</see> document. You can use this class to create and modify JSON:API data.
    /// </summary>
    /// <remarks>
    /// The <see cref="JsonApiDocument{T}"/> class is an in-memory representation of a JSON:API document. It
    /// implements the <see href="https://jsonapi.org/">JSON:API</see> v1.0 specifications.
    /// </remarks>
    /// <typeparam name="T">The type of the JSON:API document's primary data.</typeparam>
    public sealed class JsonApiDocument<T>
    {
        /// <summary>
        /// Gets the primary data of the document.
        /// </summary>
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        /// <summary>
        /// Gets an array of error objects.
        /// </summary>
        [JsonPropertyName("errors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiError[]? Errors { get; set; }

        /// <summary>
        /// Gets a meta object containing non-standard meta-information about the document.
        /// </summary>
        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }

        /// <summary>
        /// Gets an object describing the implementation characteristics of the server.
        /// </summary>
        [JsonPropertyName("jsonapi")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiObject? JsonApi { get; set; }

        /// <summary>
        /// Gets the links that relate to the primary data of the document. 
        /// </summary>
        [JsonPropertyName("links")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiDocumentLinks? Links { get; set; }

        /// <summary>
        /// Returns a new document containing the specified primary data.
        /// </summary>
        /// <remarks>
        /// You can use <see cref="Create"/> to create a JSON:API document containing your data. Jsonyte usually
        /// doesn't require this, since it can determine the serialization graph independently. However there are
        /// situations where using <see cref="Create"/> is necessary.
        ///
        /// When creating anonymous types or anonymous collections, the reflection metadata can be hidden by the
        /// compiler. This can happen if the anonymous type is created outside of the method that is used for serializing,
        /// or if the anonymous type is obtained via a method that returns <see cref="object"/> or <see cref="IEnumerable{T}"/>.
        /// In this instance, using <see cref="Create"/> forces Jsonyte to recognize your object as being valid primary data.
        ///
        /// </remarks>
        /// <example>
        /// The following example creates a new document from an anonymous resource that is hidden behind a method.
        ///
        /// <code>
        /// using Jsonyte;
        /// 
        /// class Example
        /// {
        ///     public static void Main()
        ///     {
        ///         var resources = Get();
        /// 
        ///         var document = JsonApiDocument.Create(resources);
        ///     }
        /// 
        ///     // The actual type of the resources are hidden by the compiler as an 'object'.
        ///     private static object Get()
        ///     {
        ///         return new[]
        ///         {
        ///             new
        ///             {
        ///                 id = "1",
        ///                 type = "articles",
        ///                 title = "JSON:API"
        ///             }
        ///         };
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="data">The primary data to include in the document.</param>
        /// <returns>The document containing the specified data</returns>
        public static JsonApiDocument<T> Create(T data)
        {
            return new()
            {
                Data = data
            };
        }
    }

    /// <summary>
    /// Represents a <see href="https://jsonapi.org/">JSON:API</see> document. You can use this class to create and modify JSON:API data.
    /// </summary>
    /// <remarks>
    /// The <see cref="JsonApiDocument"/> class is an in-memory representation of a JSON:API document. It
    /// implements the <see href="https://jsonapi.org/">JSON:API</see> v1.0 specifications.
    /// </remarks>
    public sealed class JsonApiDocument
    {
        /// <summary>
        /// Gets the primary data of the document.
        /// </summary>
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiResource[]? Data { get; set; }

        /// <summary>
        /// Gets an array of error objects.
        /// </summary>
        [JsonPropertyName("errors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiError[]? Errors { get; set; }

        /// <summary>
        /// Gets a meta object containing non-standard meta-information about the document.
        /// </summary>
        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }

        /// <summary>
        /// Gets an object describing the implementation characteristics of the server.
        /// </summary>
        [JsonPropertyName("jsonapi")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiObject? JsonApi { get; set; }

        /// <summary>
        /// Gets the links that relate to the primary data of the document. 
        /// </summary>
        [JsonPropertyName("links")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiDocumentLinks? Links { get; set; }

        /// <summary>
        /// Gets an array of resource objects that are related to the primary data of the document.
        /// </summary>
        [JsonPropertyName("included")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiResource[]? Included { get; set; }
        
        /// <summary>
        /// Returns a new document containing the specified primary data.
        /// </summary>
        /// <remarks>
        /// You can use <see cref="Create{T}"/> to create a JSON:API document containing your data. Jsonyte usually
        /// doesn't require this, since it can determine the serialization graph independently. However there are
        /// situations where using <see cref="Create{T}"/> is necessary.
        ///
        /// When creating anonymous types or anonymous collections, the reflection metadata can be hidden by the
        /// compiler. This can happen if the anonymous type is created outside of the method that is used for serializing,
        /// or if the anonymous type is obtained via a method that returns <see cref="object"/> or <see cref="IEnumerable{T}"/>.
        /// In this instance, using <see cref="Create{T}"/> forces Jsonyte to recognize your object as being valid primary data.
        ///
        /// </remarks>
        /// <example>
        /// The following example creates a new document from an anonymous resource that is hidden behind a method.
        ///
        /// <code>
        /// using Jsonyte;
        /// 
        /// class Example
        /// {
        ///     public static void Main()
        ///     {
        ///         var resources = Get();
        /// 
        ///         var document = JsonApiDocument.Create(resources);
        ///     }
        /// 
        ///     // The actual type of the resources are hidden by the compiler as an 'object'.
        ///     private static object Get()
        ///     {
        ///         return new[]
        ///         {
        ///             new
        ///             {
        ///                 id = "1",
        ///                 type = "articles",
        ///                 title = "JSON:API"
        ///             }
        ///         };
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <typeparam name="T">The type of the primary data in the document</typeparam>
        /// <param name="data">The primary data to include in the document.</param>
        /// <returns>The document containing the specified data</returns>
        public static JsonApiDocument<T> Create<T>(T data)
        {
            return new()
            {
                Data = data
            };
        }
    }
}
