using System;
using Newtonsoft.Json.Serialization;

namespace HttPlaceholder.Application.Infrastructure.Newtonsoft
{
    // Source: https://stackoverflow.com/questions/24143149/keep-casing-when-serializing-dictionaries
    public class CamelCaseExceptDictionaryKeysResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            var contract = base.CreateDictionaryContract(objectType);
            contract.DictionaryKeyResolver = propertyName => propertyName;
            return contract;
        }
    }
}
