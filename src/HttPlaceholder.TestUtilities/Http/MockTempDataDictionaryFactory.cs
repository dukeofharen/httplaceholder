using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HttPlaceholder.TestUtilities.Http;

internal class MockTempDataDictionaryFactory : ITempDataDictionaryFactory
{
    private readonly TempDataDictionary _tempDataDictionary = new TempDataDictionary();

    public ITempDataDictionary GetTempData(HttpContext context) => _tempDataDictionary;

    private class TempDataDictionary : Dictionary<string, object>, ITempDataDictionary
    {
        public void Keep() => throw new NotImplementedException();

        public void Keep(string key) => throw new NotImplementedException();

        public void Load() => throw new NotImplementedException();

        public object Peek(string key) => throw new NotImplementedException();

        public void Save() => throw new NotImplementedException();
    }
}