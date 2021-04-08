using System.Text.Json;

namespace JsonApi
{
    internal static class JsonApiMembers
    {
        public const string Data = "data";

        public static readonly JsonEncodedText DataEncoded = JsonEncodedText.Encode(Data);

        public const string Errors = "errors";

        public const string Meta = "meta";

        public const string Attributes = "attributes";

        public static readonly JsonEncodedText AttributesEncoded = JsonEncodedText.Encode(Attributes);

        public const string Id = "id";

        public const string Type = "type";

        public const string Relationships = "relationships";

        public static readonly JsonEncodedText RelationshipsEncoded = JsonEncodedText.Encode(Relationships);

        public const string Included = "included";

        public static readonly JsonEncodedText IncludedEncoded = JsonEncodedText.Encode(Included);

        public const string Links = "links";

        public const string JsonApi = "jsonapi";

        public const string Status = "status";

        public const string Code = "code";

        public const string Title = "title";

        public const string Detail = "detail";

        public const string Source = "source";

        public const string Href = "href";
    }
}
