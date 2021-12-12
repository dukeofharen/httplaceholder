using System.Text.RegularExpressions;
using System.Xml;

namespace HttPlaceholder.Common.Utilities;

public static class XmlUtilities
{
    private static Regex NamespaceRegex { get; } = new("xmlns:(.*?)=\"(.*?)\"", RegexOptions.Compiled);

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