using System.Collections.Generic;

namespace Jsonyte
{
    /// <summary>
    /// Represents a collection of link URLs that relate to a <see href="https://jsonapi.org/">JSON:API</see> document or member.
    /// </summary>
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
