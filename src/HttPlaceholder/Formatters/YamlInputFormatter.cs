using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Formatters;

/// <summary>
/// An input formatter that is used to handle YAML input.
/// Source: https://github.com/fiyazbinhasan/CoreFormatters
/// </summary>
public class YamlInputFormatter : TextInputFormatter
{
    private readonly IDeserializer _deserializer;

    /// <summary>
    /// Constructs a <see cref="YamlInputFormatter"/> instance.
    /// </summary>
    /// <param name="deserializer"></param>
    public YamlInputFormatter(IDeserializer deserializer)
    {
        _deserializer = deserializer;

        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
        SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationYaml);
        SupportedMediaTypes.Add(MediaTypeHeaderValues.TextYaml);
    }

    /// <inheritdoc />
    public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (encoding == null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        var request = context.HttpContext.Request;

        using var streamReader = context.ReaderFactory(request.Body, encoding);
        var type = context.ModelType;

        try
        {
            var model = _deserializer.Deserialize(streamReader, type);
            return InputFormatterResult.SuccessAsync(model);
        }
        catch (Exception)
        {
            return InputFormatterResult.FailureAsync();
        }
    }
}
