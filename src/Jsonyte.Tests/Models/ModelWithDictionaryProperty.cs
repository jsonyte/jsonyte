using System.Collections.Generic;

namespace Jsonyte.Tests.Models
{
    public class ModelWithDictionaryProperty
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public IDictionary<string, string> Dictionary { get; set; }
    }
}
