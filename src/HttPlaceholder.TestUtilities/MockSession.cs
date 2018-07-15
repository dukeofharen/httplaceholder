using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HttPlaceholder.TestUtilities
{
   public class MockSession : ISession
   {
      private readonly IDictionary<string, byte[]> _dictionary = new Dictionary<string, byte[]>();

      public Task LoadAsync(CancellationToken cancellationToken = new CancellationToken())
      {
         throw new NotImplementedException();
      }

      public Task CommitAsync(CancellationToken cancellationToken = new CancellationToken())
      {
         throw new NotImplementedException();
      }

      public bool TryGetValue(string key, out byte[] value)
      {
         return _dictionary.TryGetValue(key, out value);
      }

      public void Set(string key, byte[] value)
      {
         _dictionary.Add(key, value);
      }

      public void Remove(string key)
      {
         _dictionary.Remove(key);
      }

      public void Clear()
      {
         _dictionary.Clear();
      }

      public bool IsAvailable { get; }
      public string Id { get; }
      public IEnumerable<string> Keys => _dictionary.Keys;
   }
}
