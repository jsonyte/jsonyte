namespace JsonApi
{
    internal interface IJsonApiDocument
    {
        JsonApiError[]? Errors { get; set; }

        JsonApiMeta? Meta { get; set; }

        JsonApiObject? JsonApi { get; set; }

        JsonApiLinks? Links { get; set; }

        JsonApiResource[]? Included { get; set; }
    }
}
