using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Web.Shared.Formatters;

/// <summary>
///     An output formatter that is used to handle YAML output.
///     Source: https://github.com/fiyazbinhasan/CoreFormatters
/// </summary>
public class YamlOutputFormatter : TextOutputFormatter
{
    private readonly ISerializer _serializer;

    /// <summary>
    ///     Constructs a <see cref="YamlOutputFormatter" /> instance.
    /// </summary>
    /// <param name="serializer"></param>
    public YamlOutputFormatter(ISerializer serializer)
    {
        _serializer = serializer;

        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
        SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationYaml);
        SupportedMediaTypes.Add(MediaTypeHeaderValues.TextYaml);
    }

    /// <inheritdoc />
    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(selectedEncoding);

        var response = context.HttpContext.Response;
        await using var writer = context.WriterFactory(response.Body, selectedEncoding);
        WriteObject(writer, context.Object);

        await writer.FlushAsync(context.HttpContext.RequestAborted);
    }

    private void WriteObject(TextWriter writer, object value)
    {
        ArgumentNullException.ThrowIfNull(writer);
        _serializer.Serialize(writer, value);
    }
}
