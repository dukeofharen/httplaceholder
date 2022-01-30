using System.IO;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
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
/// Response writer that is used to generate a random image (BMP, GIF, JPEG or PNG) with parameters (like quality, size etc.) and return it to the client.
/// </summary>
internal class ImageResponseWriter : IResponseWriter
{
    private readonly IAssemblyService _assemblyService;
    private readonly IFileService _fileService;

    public ImageResponseWriter(IAssemblyService assemblyService, IFileService fileService)
    {
        _assemblyService = assemblyService;
        _fileService = fileService;
    }

    /// <inheritdoc />
    public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
    {
        var imgDefinition = stub.Response?.Image;
        if (imgDefinition == null || imgDefinition.Type == ResponseImageType.NotSet)
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        var stubImage = stub.Response.Image;
        response.Headers.AddOrReplaceCaseInsensitive("Content-Type", stubImage.ContentTypeHeaderValue);

        var cacheFilePath = Path.Combine(_fileService.GetTempPath(), $"{stubImage.Hash}.bin");
        byte[] bytes;
        if (_fileService.FileExists(cacheFilePath))
        {
            bytes = _fileService.ReadAllBytes(cacheFilePath);
        }
        else
        {
            var collection = new FontCollection();
            collection.Install(Path.Combine(_assemblyService.GetExecutingAssemblyRootPath(),
                "Manrope-Regular.ttf"));
            const string fontFamilyName = "Manrope";
            if (!collection.TryFind(fontFamilyName, out var family))
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
                    await image.SaveAsBmpAsync(ms);
                    break;
                case ResponseImageType.Gif:
                    await image.SaveAsGifAsync(ms);
                    break;
                case ResponseImageType.Jpeg:
                    await image.SaveAsJpegAsync(ms, new JpegEncoder {Quality = stubImage.JpegQuality});
                    break;
                default:
                    await image.SaveAsPngAsync(ms);
                    break;
            }

            bytes = ms.ToArray();
            _fileService.WriteAllBytes(cacheFilePath, bytes);
        }

        response.Body = bytes;
        return StubResponseWriterResultModel.IsExecuted(GetType().Name);
    }

    /// <inheritdoc />
    public int Priority => -11;
}
