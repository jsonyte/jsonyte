﻿namespace Jsonyte.Tests.Models
{
    public class ModelWithCircularTypeCollectionNested
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public ModelWithCircularTypeCollection Second { get; set; }
    }
}
