using System.Collections;
using System.Collections.Generic;

namespace Jsonyte.Tests.Data
{
    public class ModelWithCollectionInterfacesData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {new Dictionary<string, string>(), new List<string>(), new List<string>()};
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
