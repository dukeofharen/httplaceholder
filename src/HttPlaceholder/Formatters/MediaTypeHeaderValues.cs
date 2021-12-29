using HttPlaceholder.Domain;
using Microsoft.Net.Http.Headers;

namespace HttPlaceholder.Formatters;

internal static class MediaTypeHeaderValues
{
    public static readonly MediaTypeHeaderValue ApplicationYaml = MediaTypeHeaderValue.Parse(Constants.YamlApplicationMime).CopyAsReadOnly();

    public static readonly MediaTypeHeaderValue TextYaml = MediaTypeHeaderValue.Parse(Constants.YamlTextMime).CopyAsReadOnly();

    public static readonly MediaTypeHeaderValue PlainText = MediaTypeHeaderValue.Parse(Constants.TextMime).CopyAsReadOnly();
}
