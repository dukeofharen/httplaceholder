using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace HttPlaceholder.Formatters;

public class PlainTextInputFormatter : TextInputFormatter
{
    public PlainTextInputFormatter()
    {
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
        SupportedMediaTypes.Add(MediaTypeHeaderValues.PlainText);
    }

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
        try
        {
            var text = await streamReader.ReadToEndAsync();
            return await InputFormatterResult.SuccessAsync(text);
        }
        catch (Exception)
        {
            return await InputFormatterResult.FailureAsync();
        }
    }
}