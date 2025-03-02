using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace HttPlaceholder.TestUtilities.Http;

internal class MockHeaderDictionary : Dictionary<string, StringValues>, IHeaderDictionary
{
    public long? ContentLength { get; set; }
}
