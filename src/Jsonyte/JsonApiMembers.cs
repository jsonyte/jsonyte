using System.Text.Json;

namespace Jsonyte
{
    internal static class JsonApiMembers
    {
        public const string Data = "data";

        public static readonly JsonEncodedText DataEncoded = JsonEncodedText.Encode(Data);

        public static readonly ulong DataKey = DataEncoded.EncodedUtf8Bytes.GetKey();

        public const string Errors = "errors";

        public static readonly JsonEncodedText ErrorsEncoded = JsonEncodedText.Encode(Errors);

        public static readonly ulong ErrorsKey = ErrorsEncoded.EncodedUtf8Bytes.GetKey();

        public const string Meta = "meta";

        public static readonly JsonEncodedText MetaEncoded = JsonEncodedText.Encode(Meta);

        public static readonly ulong MetaKey = MetaEncoded.EncodedUtf8Bytes.GetKey();

        public const string Attributes = "attributes";

        public static readonly JsonEncodedText AttributesEncoded = JsonEncodedText.Encode(Attributes);

        public static readonly ulong AttributesKey = AttributesEncoded.EncodedUtf8Bytes.GetKey();

        public const string Id = "id";

        public static readonly JsonEncodedText IdEncoded = JsonEncodedText.Encode(Id);

        public static readonly ulong IdKey = IdEncoded.EncodedUtf8Bytes.GetKey();

        public const string Type = "type";

        public static readonly JsonEncodedText TypeEncoded = JsonEncodedText.Encode(Type);

        public static readonly ulong TypeKey = TypeEncoded.EncodedUtf8Bytes.GetKey();

        public const string Relationships = "relationships";

        public static readonly JsonEncodedText RelationshipsEncoded = JsonEncodedText.Encode(Relationships);

        public static readonly ulong RelationshipsKey = RelationshipsEncoded.EncodedUtf8Bytes.GetKey();

        public const string Included = "included";

        public static readonly JsonEncodedText IncludedEncoded = JsonEncodedText.Encode(Included);

        public static readonly ulong IncludedKey = IncludedEncoded.EncodedUtf8Bytes.GetKey();

        public const string Links = "links";

        public static readonly JsonEncodedText LinksEncoded = JsonEncodedText.Encode(Links);

        public static readonly ulong LinksKey = LinksEncoded.EncodedUtf8Bytes.GetKey();

        public const string JsonApi = "jsonapi";

        public static readonly JsonEncodedText JsonApiEncoded = JsonEncodedText.Encode(JsonApi);

        public static readonly ulong JsonApiKey = JsonApiEncoded.EncodedUtf8Bytes.GetKey();

        public const string Status = "status";

        public static readonly JsonEncodedText StatusEncoded = JsonEncodedText.Encode(Status);

        public static readonly ulong StatusKey = StatusEncoded.EncodedUtf8Bytes.GetKey();

        public const string Code = "code";

        public static readonly JsonEncodedText CodeEncoded = JsonEncodedText.Encode(Code);

        public static readonly ulong CodeKey = CodeEncoded.EncodedUtf8Bytes.GetKey();

        public const string Title = "title";

        public static readonly JsonEncodedText TitleEncoded = JsonEncodedText.Encode(Title);

        public static readonly ulong TitleKey = TitleEncoded.EncodedUtf8Bytes.GetKey();

        public const string Detail = "detail";

        public static readonly JsonEncodedText DetailEncoded = JsonEncodedText.Encode(Detail);

        public static readonly ulong DetailKey = DetailEncoded.EncodedUtf8Bytes.GetKey();

        public const string Source = "source";

        public static readonly JsonEncodedText SourceEncoded = JsonEncodedText.Encode(Source);

        public static readonly ulong SourceKey = SourceEncoded.EncodedUtf8Bytes.GetKey();

        public const string Href = "href";

        public static readonly JsonEncodedText HrefEncoded = JsonEncodedText.Encode(Href);

        public static readonly ulong HrefKey = HrefEncoded.EncodedUtf8Bytes.GetKey();
    }
}
