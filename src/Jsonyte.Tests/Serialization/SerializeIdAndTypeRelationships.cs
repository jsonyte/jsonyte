using Jsonyte.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Jsonyte.Tests.Serialization
{
    public class SerializeIdAndTypeRelationships
    {
        [Fact]
        public void CanSerializeModelWithAnonymousCollectionIdAndTypeRelationship() {
            var model = new ModelWithAnonymousCollectionIdAndTypeRelationship
            {
                Id = "1",
                AnonymousCollection = new List<ModelWithAttribute>
                {
                    new ModelWithAttribute
                    {
                        Id = "2",
                        Value = "2",
                        IntValue = 2
                    },
                    new ModelWithAttribute
                    {
                        Id = "3",
                        Value = "3",
                        IntValue = 3
                    }
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model-with-anonymous-collection-id-and-type-relationship',
                    'relationships': {
                      'anonymousCollection': {
                        'data': [
                          {
                            'id': '2',
                            'type': 'model-with-attribute'
                          },
                          {
                            'id': '3',
                            'type': 'model-with-attribute'
                          }
                        ]
                      }
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelWithExplicitAnonymousCollectionIdAndTypeRelationship() {
            var model = new ModelWithExplicitAnonymousCollectionIdAndTypeRelationship
            {
                Id = "1",
                ExplicitAnonymousCollection = new JsonApiRelationship<IEnumerable<ModelWithAttribute>>
                {
                    Data = new List<ModelWithAttribute>
                    {
                        new ModelWithAttribute
                        {
                            Id = "2",
                            Value = "2",
                            IntValue = 2
                        },
                        new ModelWithAttribute
                        {
                            Id = "3",
                            Value = "3",
                            IntValue = 3
                        }
                    }
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model-with-explicit-anonymous-collection-id-and-type-relationship',
                    'relationships': {
                      'explicitAnonymousCollection': {
                        'data': [
                          {
                            'id': '2',
                            'type': 'model-with-attribute'
                          },
                          {
                            'id': '3',
                            'type': 'model-with-attribute'
                          }
                        ]
                      }
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelWithPotentialCollectionIdAndTypeRelationship(){
            var model = new ModelWithPotentialCollectionIdAndTypeRelationship
            {
                Id = "1",
                PotentialCollection = new List<object>
                {
                    new
                    {
                        id = "2",
                        type = "model-with-attribute",
                        value = "2",
                        intValue = 2
                    },
                    new
                    {
                        id = "3",
                        type = "model-with-attribute",
                        value = "3",
                        intValue = 3
                    }
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model-with-potential-collection-id-and-type-relationship',
                    'relationships': {
                      'potentialCollection': {
                        'data': [
                          {
                            'id': '2',
                            'type': 'model-with-attribute'
                          },
                          {
                            'id': '3',
                            'type': 'model-with-attribute'
                          }
                        ]
                      }
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelWithExplicitPotentialCollectionIdAndTypeRelationship(){
            var model = new ModelWithExplicitPotentialCollectionIdAndTypeRelationship
            {
                Id = "1",
                ExplicitPotentialCollection = new JsonApiRelationship<IEnumerable<object>>
                {
                    Data = new List<object>
                    {
                        new
                        {
                            id = "2",
                            type = "model-with-attribute",
                            value = "2",
                            intValue = 2
                        },
                        new
                        {
                            id = "3",
                            type = "model-with-attribute",
                            value = "3",
                            intValue = 3
                        }
                    }
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model-with-explicit-potential-collection-id-and-type-relationship',
                    'relationships': {
                      'explicitPotentialCollection': {
                        'data': [
                          {
                            'id': '2',
                            'type': 'model-with-attribute'
                          },
                          {
                            'id': '3',
                            'type': 'model-with-attribute'
                          }
                        ]
                      }
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelWithObjectIdAndTypeRelationship() {
             var model = new ModelWithObjectIdAndTypeRelationship()
             {
                 Id = "1",
                 Object = new
                 {
                     id = "2",
                     type = "model-with-attribute",
                     value = "2",
                     intValue = 2,
                     Object = new ModelWithAttribute
                     {
                         Id = "2",
                         Value = "2",
                         IntValue = 2
                     }
                 }
             };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model-with-object-id-and-type-relationship',
                    'relationships': {
                      'object': {
                        'data': {
                          'id': '2',
                          'type': 'model-with-attribute'
                        }
                      }
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelReferencingItselfWithIdAndTypeRelationship()
        {
            var model = new ModelReferencingItselfWithIdAndTypeRelationship
            {
                Id = "1",
                Value = "2",
            };

            model.Itself =  model;

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model-referencing-itself-with-id-and-type-relationship',
                    'attributes': {
                      'value': '2'
                    },
                    'relationships': {
                      'itself': {
                        'data': {
                          'id': '1',
                          'type': 'model-referencing-itself-with-id-and-type-relationship'
                        }
                      }
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelWithArrayIdAndTypeRelationship()
        {
            var model = new ModelWithArrayIdAndTypeRelationship
            {
                Id = "1",
                Array = new[]
                {
                    new ModelWithAttribute
                    {
                        Id = "2",
                        Value = "2",
                        IntValue = 2
                    },
                    new ModelWithAttribute
                    {
                        Id = "3",
                        Value = "3",
                        IntValue = 3
                    }
                }
            };
            var json = model.Serialize();
            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model-with-array-id-and-type-relationship',
                    'relationships': {
                      'array': {
                        'data': [
                          {
                            'id': '2',
                            'type': 'model-with-attribute'
                          },
                          {
                            'id': '3',
                            'type': 'model-with-attribute'
                          }
                        ]
                      }
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelWithExplicitArrayIdAndTypeRelationship()
        {
            var model = new ModelWithExplicitArrayIdAndTypeRelationship
            {
                Id = "1",
                ExplicitArray = new JsonApiRelationship<ModelWithAttribute[]>
                {
                    Data = new[]
                    {
                        new ModelWithAttribute
                        {
                            Id = "2",
                            Value = "2",
                            IntValue = 2
                        },
                        new ModelWithAttribute
                        {
                            Id = "3",
                            Value = "3",
                            IntValue = 3
                        }
                    }
                }
            };
            var json = model.Serialize();
            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model-with-explicit-array-id-and-type-relationship',
                    'relationships': {
                      'explicitArray': {
                        'data': [
                          {
                            'id': '2',
                            'type': 'model-with-attribute'
                          },
                          {
                            'id': '3',
                            'type': 'model-with-attribute'
                          }
                        ]
                      }
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelWithExplicitIdAndTypeRelationship()
        {
            var model = new ModelWithExplicitIdAndTypeRelationship
            {
                Id = "1",
                ExplicitModel = new JsonApiRelationship<ModelWithAttribute>
                {
                    Data = new ModelWithAttribute
                    {
                        Id = "2",
                        Value = "2",
                        IntValue = 2
                    }
                }
            };
            var json = model.Serialize();
            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model-with-explicit-id-and-type-relationship',
                    'relationships': {
                      'explicitModel': {
                        'data': {
                          'id': '2',
                          'type': 'model-with-attribute'
                        }
                      }
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelWithExplicitObjectIdAndTypeRelationship()
        {
            var model = new ModelWithExplicitObjectIdAndTypeRelationship
            {
                Id = "1",
                ExplicitObject = new JsonApiRelationship<object>
                {
                    Data = new
                    {
                        id = "2",
                        type = "model-with-attribute",
                        value = "2",
                        intValue = 2,
                        Object = new ModelWithAttribute
                        {
                            Id = "3",
                            Value = "3",
                            IntValue = 3
                        }
                    }
                }
            };

            var json = model.Serialize();
            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model-with-explicit-object-id-and-type-relationship',
                    'relationships': {
                      'explicitObject': {
                        'data': {
                          'id': '2',
                          'type': 'model-with-attribute'
                        }
                      }
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelWithIdAndTypeRelationship()
        {
            var model = new ModelWithIdAndTypeRelationship
            {
                Id = "1",
                Model = new ModelWithAttribute
                {
                    Id = "2",
                    Value = "2",
                    IntValue = 2
                }
            };
            var json = model.Serialize();
            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model-with-id-and-type-relationship',
                    'relationships': {
                      'model': {
                        'data': {
                          'id': '2',
                          'type': 'model-with-attribute'
                        }
                      }
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void ModelWithNestedIdAndTypeRelationship()
        {
            var model = new ModelWithNestedIdAndTypeRelationship
            {
                Id = "1",
                Model = new ModelWithIdAndTypeRelationship
                {
                    Id = "2",
                    Model = new ModelWithAttribute
                    {
                        Id = "3",
                        Value = "3",
                        IntValue = 3
                    }
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model-with-nested-id-and-type-relationship',
                    'relationships': {
                      'model': {
                        'data': {
                          'id': '2',
                          'type': 'model-with-id-and-type-relationship'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '2',
                      'type': 'model-with-id-and-type-relationship',
                      'relationships': {
                        'model': {
                          'data': {
                            'id': '3',
                            'type': 'model-with-attribute'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelWithIdAndTypeRelationshipCollection()
        {
            var model = new List<ModelWithIdAndTypeRelationship>
            {
                new ModelWithIdAndTypeRelationship {
                    Id = "1",
                    Model = new ModelWithAttribute
                    {
                        Id = "2",
                        Value = "2",
                        IntValue = 2
                    }
                },
                new ModelWithIdAndTypeRelationship {
                    Id = "3",
                    Model = new ModelWithAttribute
                    {
                        Id = "4",
                        Value = "4",
                        IntValue = 4
                    }
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'model-with-id-and-type-relationship',
                      'relationships': {
                        'model': {
                          'data': {
                            'id': '2',
                            'type': 'model-with-attribute'
                          }
                        }
                      }
                    },
                    {
                      'id': '3',
                      'type': 'model-with-id-and-type-relationship',
                      'relationships': {
                        'model': {
                          'data': {
                            'id': '4',
                            'type': 'model-with-attribute'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelWithUselessIdAndTypeRelationship()
        {
            var model = new ModelWithUselessIdAndTypeRelationship
            {
                Id = "1",
                Value = "2"
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model-with-useless-id-and-type-relationship',
                    'attributes': {
                      'value': '2'
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }
    }
}
