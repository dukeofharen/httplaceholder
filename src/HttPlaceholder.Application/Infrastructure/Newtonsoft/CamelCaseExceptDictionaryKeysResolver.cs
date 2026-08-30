using System;
using Newtonsoft.Json.Serialization;

namespace HttPlaceholder.Application.Infrastructure.Newtonsoft;

// Source: https://stackoverflow.com/questions/24143149/keep-casing-when-serializing-dictionaries
/// <summary>
///     A <see cref="CamelCasePropertyNamesContractResolver" /> for leaving the property names as is when serializing a
///     dictionary.
/// </summary>
public class CamelCaseExceptDictionaryKeysResolver : CamelCasePropertyNamesContractResolver
{
    /// <inheritdoc />
    protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
    {
        var contract = base.CreateDictionaryContract(objectType);
        contract.DictionaryKeyResolver = propertyName => propertyName;
        return contract;
    }
}
