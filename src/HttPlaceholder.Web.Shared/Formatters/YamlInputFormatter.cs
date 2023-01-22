using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Web.Shared.Formatters;

/// <summary>
///     An input formatter that is used to handle YAML input.
///     Source: https://github.com/fiyazbinhasan/CoreFormatters
/// </summary>
public class YamlInputFormatter : TextInputFormatter
{
    private readonly IDeserializer _deserializer;

    /// <summary>
    ///     Constructs a <see cref="YamlInputFormatter" /> instance.
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
    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context,
        Encoding encoding)
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
            return await InputFormatterResult.SuccessAsync(model);
        }
        catch (Exception ex)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<YamlInputFormatter>>();
            logger.LogWarning(ex, "Error occurred while deserializing YAML.");
            context.ModelState.AddModelError("yaml", ex.Message);
            return await InputFormatterResult.FailureAsync();
        }
    }
}
