using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.TestUtilities.Http
{
    public class RequestCookieCollection : Dictionary<string, string>, IRequestCookieCollection
    {
        public RequestCookieCollection(IDictionary<string, string> dict)
        {
            foreach (var (key, value) in dict)
            {
                Add(key, value);
            }
        }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public new ICollection<string> Keys { get; }
    }
}
