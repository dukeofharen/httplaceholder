using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HttPlaceholder.TestUtilities
{
    public class MockTempDataDictionaryFactory : ITempDataDictionaryFactory
    {
        private readonly TempDataDictionary _tempDataDictionary = new TempDataDictionary();

        public ITempDataDictionary GetTempData(HttpContext context)
        {
            return _tempDataDictionary;
        }

        private class TempDataDictionary : ITempDataDictionary
        {
            private readonly IDictionary<string, object> _dictionary = new Dictionary<string, object>();

            public void Load()
            {
                throw new NotImplementedException();
            }

            public void Save()
            {
                throw new NotImplementedException();
            }

            public void Keep()
            {
                throw new NotImplementedException();
            }

            public void Keep(string key)
            {
                throw new NotImplementedException();
            }

            public object Peek(string key)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public void Add(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public int Count { get; }
            public bool IsReadOnly { get; }

            public void Add(string key, object value)
            {
                throw new NotImplementedException();
            }

            public bool ContainsKey(string key)
            {
                throw new NotImplementedException();
            }

            public bool Remove(string key)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(string key, out object value)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public object this[string key]
            {
                get
                {
                    _dictionary.TryGetValue(key, out object value);
                    return value;
                }
                set
                {
                    _dictionary.Remove(key);
                    _dictionary.Add(key, value);
                }
            }

            public ICollection<string> Keys { get; }
            public ICollection<object> Values { get; }
        }
    }
}