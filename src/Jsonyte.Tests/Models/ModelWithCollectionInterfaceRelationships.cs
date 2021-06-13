using System.Collections.Generic;

namespace Jsonyte.Tests.Models
{
    public class ModelWithCollectionInterfaceRelationships
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}
