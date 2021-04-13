using System.Text.Json;

namespace JsonApi
{
    internal static class JsonApiMembers
    {
        public const string Data = "data";

        public static readonly JsonEncodedText DataEncoded = JsonEncodedText.Encode(Data);

        public const string Errors = "errors";

        public static readonly JsonEncodedText ErrorsEncoded = JsonEncodedText.Encode(Errors);

        public const string Meta = "meta";

        public static readonly JsonEncodedText MetaEncoded = JsonEncodedText.Encode(Meta);

        public const string Attributes = "attributes";

        public static readonly JsonEncodedText AttributesEncoded = JsonEncodedText.Encode(Attributes);

        public const string Id = "id";

        public static readonly JsonEncodedText IdEncoded = JsonEncodedText.Encode(Id);

        public const string Type = "type";

        public static readonly JsonEncodedText TypeEncoded = JsonEncodedText.Encode(Type);

        public const string Relationships = "relationships";

        public static readonly JsonEncodedText RelationshipsEncoded = JsonEncodedText.Encode(Relationships);

        public const string Included = "included";

        public static readonly JsonEncodedText IncludedEncoded = JsonEncodedText.Encode(Included);

        public const string Links = "links";

        public static readonly JsonEncodedText LinksEncoded = JsonEncodedText.Encode(Links);

        public const string JsonApi = "jsonapi";

        public static readonly JsonEncodedText JsonApiEncoded = JsonEncodedText.Encode(JsonApi);

        public const string Status = "status";

        public static readonly JsonEncodedText StatusEncoded = JsonEncodedText.Encode(Status);

        public const string Code = "code";

        public static readonly JsonEncodedText CodeEncoded = JsonEncodedText.Encode(Code);

        public const string Title = "title";

        public static readonly JsonEncodedText TitleEncoded = JsonEncodedText.Encode(Title);

        public const string Detail = "detail";

        public static readonly JsonEncodedText DetailEncoded = JsonEncodedText.Encode(Detail);

        public const string Source = "source";

        public static readonly JsonEncodedText SourceEncoded = JsonEncodedText.Encode(Source);

        public const string Href = "href";

        public static readonly JsonEncodedText HrefEncoded = JsonEncodedText.Encode(Href);
    }
}
