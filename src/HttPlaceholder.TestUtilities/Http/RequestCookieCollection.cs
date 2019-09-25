using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.TestUtilities.Http
{
    public class RequestCookieCollection : Dictionary<string, string>, IRequestCookieCollection
    {
        public RequestCookieCollection(IDictionary<string, string> dict)
        {
            foreach (var pair in dict)
            {
                Add(pair.Key, pair.Value);
            }
        }

        public ICollection<string> Keys { get; }
    }
}
