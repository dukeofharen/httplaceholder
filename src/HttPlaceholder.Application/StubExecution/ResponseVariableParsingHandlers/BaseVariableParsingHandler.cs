using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Describes a class that is used to read a response body and parse the variables.
///     A variable in the response body can be in the form of "((variable))" or "((variable:parameter))".
/// </summary>
internal abstract class BaseVariableParsingHandler : IResponseVariableParsingHandler
{
    private readonly Lazy<string> _loadedDescription;

    protected BaseVariableParsingHandler(IFileService fileService)
    {
        _loadedDescription = new Lazy<string>(() =>
        {
            var path = Path.Combine(AssemblyHelper.GetExecutingAssemblyRootPath(), "Files", "VarParser",
                $"{Name}-description.md");
            return fileService.ReadAllText(path);
        });
    }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public abstract string FullName { get; }

    /// <inheritdoc />
    public virtual string GetDescription() => _loadedDescription.Value;

    /// <inheritdoc />
    public abstract string[] Examples { get; }

    /// <inheritdoc />
    public string Parse(string input, IEnumerable<Match> matches, StubModel stub)
    {
        var enumerable = matches as Match[] ?? matches.ToArray();
        if (!enumerable.Any())
        {
            return input;
        }

        return InsertVariables(input, enumerable, stub);
    }

    /// <summary>
    ///     Inserts the given matches inside the given input.
    /// </summary>
    /// <param name="input">The response body.</param>
    /// <param name="matches">The regex matches that have been found for this variable.</param>
    /// <param name="stub">The matched stub.</param>
    /// <returns>The parsed response body.</returns>
    protected abstract string InsertVariables(string input, Match[] matches, StubModel stub);
}
