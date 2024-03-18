using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to generate a random image (BMP, GIF, JPEG or PNG) with parameters (like quality, size
///     etc.) and return it to the client.
/// </summary>
internal class ImageResponseWriter(IAssemblyService assemblyService, IFileService fileService)
    : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        var imgDefinition = stub.Response?.Image;
        if (imgDefinition == null || imgDefinition.Type == ResponseImageType.NotSet)
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        var stubImage = stub.Response.Image;
        response.Headers.AddOrReplaceCaseInsensitive(HeaderKeys.ContentType, stubImage.ContentTypeHeaderValue);

        var cacheFilePath = Path.Combine(fileService.GetTempPath(), $"{stubImage.Hash}.bin");
        byte[] bytes;
        if (await fileService.FileExistsAsync(cacheFilePath, cancellationToken))
        {
            bytes = await fileService.ReadAllBytesAsync(cacheFilePath, cancellationToken);
        }
        else
        {
            var collection = new FontCollection();
            collection.Add(Path.Combine(assemblyService.GetExecutingAssemblyRootPath(),
                "Files", "Manrope-Regular.ttf"));
            const string fontFamilyName = "Manrope";
            if (!collection.TryGet(fontFamilyName, out var family))
            {
                throw new RequestValidationException($"Font family '{fontFamilyName}' not found!");
            }

            using var image = new Image<Rgba32>(stubImage.Width, stubImage.Height);
            var font = new Font(family, stubImage.FontSize);
            var parsedColor = Color.ParseHex(stubImage.BackgroundColor);
            var polygon = new Rectangle(0, 0, stubImage.Width, stubImage.Height);
            var fontColor = !string.IsNullOrWhiteSpace(stubImage.FontColor)
                ? Color.ParseHex(stubImage.FontColor)
                : parsedColor.InvertColor();
            image.Mutate(i =>
                i
                    .Fill(parsedColor, polygon)
                    .ApplyScalingWaterMark(font, stubImage.Text, fontColor, 5, stubImage.WordWrap));
            using var ms = new MemoryStream();
            switch (stubImage.Type)
            {
                case ResponseImageType.Bmp:
                    await image.SaveAsBmpAsync(ms, cancellationToken);
                    break;
                case ResponseImageType.Gif:
                    await image.SaveAsGifAsync(ms, cancellationToken);
                    break;
                case ResponseImageType.Jpeg:
                    await image.SaveAsJpegAsync(ms, new JpegEncoder { Quality = stubImage.JpegQuality },
                        cancellationToken);
                    break;
                default:
                    await image.SaveAsPngAsync(ms, cancellationToken);
                    break;
            }

            bytes = ms.ToArray();
            await fileService.WriteAllBytesAsync(cacheFilePath, bytes, cancellationToken);
        }

        response.Body = bytes;
        response.BodyIsBinary = bytes.Length != 0;
        return StubResponseWriterResultModel.IsExecuted(GetType().Name);
    }

    /// <inheritdoc />
    public int Priority => -11;
}
