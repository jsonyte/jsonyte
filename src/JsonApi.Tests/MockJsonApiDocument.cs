namespace JsonApi.Tests
{
    public class MockJsonApiDocument
    {
        public object Data { get; set; }

        public JsonApiError[] Errors { get; set; }

        public JsonApiMeta Meta { get; set; }

        public JsonApiObject JsonApi { get; set; }

        public JsonApiDocumentLinks Links { get; set; }

        public JsonApiResource[] Included { get; set; }
    }
}
