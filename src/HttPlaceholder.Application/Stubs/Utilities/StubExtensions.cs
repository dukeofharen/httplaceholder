﻿using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.Stubs.Utilities;

/// <summary>
///     Utility class that contains several methods for working with stub models.
/// </summary>
public static class StubExtensions
{
    /// <summary>
    ///     Sets an autogenerated ID on a stub if no ID has been set yet.
    /// </summary>
    /// <param name="stub">The stub.</param>
    /// <param name="stubIdPrefix">A prefix that should be added to the stub ID.</param>
    /// <returns>The stub ID.</returns>
    public static string EnsureStubId(this StubModel stub, string stubIdPrefix = "")
    {
        var id = !string.IsNullOrWhiteSpace(stub.Id) ? stub.Id : $"generated-{HashingUtilities.GetMd5String(JsonConvert.SerializeObject(stub))}";
        if (!string.IsNullOrWhiteSpace(stubIdPrefix))
        {
            id = $"{stubIdPrefix}{id}";
        }

        stub.Id = id;
        return id;
    }
}
