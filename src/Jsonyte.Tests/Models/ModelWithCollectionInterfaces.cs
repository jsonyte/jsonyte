using System.Collections;
using System.Collections.Generic;

namespace Jsonyte.Tests.Models
{
    public class ModelWithCollectionInterfaces
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public IDictionary Dictionary { get; set; }

        public IDictionary<string, string> GenericDictionary { get; set; }

        public ICollection Collection { get; set; }

        public ICollection<string> GenericCollection { get; set; }

        public IList List { get; set; }

        public IList<string> GenericList { get; set; }
    }
}
