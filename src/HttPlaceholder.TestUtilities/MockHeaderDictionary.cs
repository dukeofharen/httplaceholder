using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace HttPlaceholder.TestUtilities
{
   public class MockHeaderDictionary : Dictionary<string, StringValues>, IHeaderDictionary
   {
      public long? ContentLength { get; set; }
   }
}
