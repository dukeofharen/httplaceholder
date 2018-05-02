using System.Collections.Generic;

namespace Placeholder.Implementation.Implementations
{
   public class StubContainer : IStubContainer
   {
      public StubContainer(IList<object> stubs)
      {
         Stubs = stubs;
      }

      public IList<object> Stubs { get; }
   }
}
