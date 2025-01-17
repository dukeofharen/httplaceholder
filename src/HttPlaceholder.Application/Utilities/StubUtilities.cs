﻿using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Utilities;

/// <summary>
///     A static class that contains several stub related utility methods.
/// </summary>
public static class StubUtilities
{
    /// <summary>
    ///     Cleans the ID of the given stub.
    /// </summary>
    /// <param name="stub">The stub.</param>
    public static void CleanStubId(this StubModel stub)
    {
        if (stub?.Id != null)
        {
            stub.Id = CleanStubId(stub.Id);
        }
    }

    /// <summary>
    ///     Cleans the stub ID.
    /// </summary>
    /// <param name="id">The stub ID.</param>
    /// <returns>The cleaned stub ID.</returns>
    public static string CleanStubId(string id) => PathUtilities.CleanPath(id);

    /// <summary>
    ///     Cleans the scenario name.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <returns>The cleaned scenario name.</returns>
    public static string CleanScenarioName(string scenario) => PathUtilities.CleanPath(scenario);

    /// <summary>
    ///     Returns the set content type of the stub.
    /// </summary>
    /// <param name="stubModel">The stub.</param>
    /// <returns>The content type or NULL if it was not found.</returns>
    public static string GetContentType(this StubModel stubModel) =>
        !string.IsNullOrWhiteSpace(stubModel.Response?.ContentType)
            ? stubModel.Response?.ContentType
            : stubModel.Response?.Headers?.CaseInsensitiveSearch(HeaderKeys.ContentType);
}
