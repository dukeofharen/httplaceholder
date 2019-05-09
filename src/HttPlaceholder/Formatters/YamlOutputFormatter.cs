using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Formatters
{
    /// <summary>
    /// Source: https://github.com/fiyazbinhasan/CoreFormatters
    /// </summary>
    public class YamlOutputFormatter : TextOutputFormatter
    {
        private readonly ISerializer _serializer;

        /// <summary>
        /// 
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="selectedEncoding"></param>
        /// <returns></returns>
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (selectedEncoding == null)
            {
                throw new ArgumentNullException(nameof(selectedEncoding));
            }

            var response = context.HttpContext.Response;
            using (var writer = context.WriterFactory(response.Body, selectedEncoding))
            {
                WriteObject(writer, context.Object);

                await writer.FlushAsync();
            }
        }

        private void WriteObject(TextWriter writer, object value)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            _serializer.Serialize(writer, value);
        }
    }
}