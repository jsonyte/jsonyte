namespace Jsonyte
{
    internal interface IJsonApiDocument
    {
        JsonApiError[]? Errors { get; set; }

        JsonApiMeta? Meta { get; set; }

        JsonApiObject? JsonApi { get; set; }

        JsonApiDocumentLinks? Links { get; set; }
    }
}
