using Microsoft.Net.Http.Headers;

namespace HttPlaceholder.Formatters;

internal static class MediaTypeHeaderValues
{
    public static readonly MediaTypeHeaderValue ApplicationYaml = MediaTypeHeaderValue.Parse("application/x-yaml").CopyAsReadOnly();

    public static readonly MediaTypeHeaderValue TextYaml = MediaTypeHeaderValue.Parse("text/yaml").CopyAsReadOnly();

    public static readonly MediaTypeHeaderValue PlainText = MediaTypeHeaderValue.Parse("text/plain").CopyAsReadOnly();
}