using System.Text.RegularExpressions;
using System.Xml;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A utility class for working with XML.
/// </summary>
public static partial class XmlUtilities
{
    [GeneratedRegex("xmlns:(.*?)=\"(.*?)\"", RegexOptions.Compiled)]
    private static partial Regex CompiledNamespaceRegex();

    private static Regex NamespaceRegex { get; } = CompiledNamespaceRegex();

    /// <summary>
    ///     Parses a given body and assign the found namespaces to the <see cref="XmlNamespaceManager" />.
    /// </summary>
    /// <param name="nsManager">The <see cref="XmlNamespaceManager" />.</param>
    /// <param name="body">The XML body.</param>
    public static void ParseBodyAndAssignNamespaces(this XmlNamespaceManager nsManager, string body)
    {
        var matches = NamespaceRegex.Matches(body);
        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];

            // If there is a match, the regex should contain three groups: the full match, the prefix and the namespace
            if (match.Groups.Count != 3)
            {
                continue;
            }

            var prefix = match.Groups[1].Value;
            var uri = match.Groups[2].Value;
            nsManager.AddNamespace(prefix, uri);
        }
    }
}
