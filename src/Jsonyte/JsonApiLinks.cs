using System.Collections.Generic;

namespace JsonApi
{
    public class JsonApiLinks : Dictionary<string, JsonApiLink>
    {
        internal JsonApiLink? GetOrNull(string key)
        {
            return TryGetValue(key, out var value)
                ? value
                : null;
        }

        internal void SetOrRemove(string key, JsonApiLink? link)
        {
            if (link != null)
            {
                base[key] = link;
            }
            else
            {
                Remove(key);
            }
        }
    }
}
