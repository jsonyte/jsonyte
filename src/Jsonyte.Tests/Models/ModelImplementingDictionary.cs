using System.Collections.Generic;

namespace Jsonyte.Tests.Models
{
    public class ModelImplementingDictionary : Dictionary<string, string>
    {
        public string Id { get; set; }

        public string Type { get; set; }
    }
}
