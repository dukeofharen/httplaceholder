using HttPlaceholder.Domain;
using Microsoft.Net.Http.Headers;

namespace HttPlaceholder.Formatters;

internal static class MediaTypeHeaderValues
{
    public static readonly MediaTypeHeaderValue ApplicationYaml =
        MediaTypeHeaderValue.Parse(MimeTypes.YamlApplicationMime).CopyAsReadOnly();

    public static readonly MediaTypeHeaderValue TextYaml =
        MediaTypeHeaderValue.Parse(MimeTypes.YamlTextMime).CopyAsReadOnly();

    public static readonly MediaTypeHeaderValue PlainText =
        MediaTypeHeaderValue.Parse(MimeTypes.TextMime).CopyAsReadOnly();
}
