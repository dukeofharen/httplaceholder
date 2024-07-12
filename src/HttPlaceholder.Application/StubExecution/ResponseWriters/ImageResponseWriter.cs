using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
using static HttPlaceholder.Domain.StubResponseWriterResultModel;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to generate a random image (BMP, GIF, JPEG or PNG) with parameters (like quality, size
///     etc.) and return it to the client.
/// </summary>
internal class ImageResponseWriter(
    IAssemblyService assemblyService,
    IFileService fileService,
    IEnvService envService)
    : IResponseWriter, ISingletonService
{
    private const bool UseImageSharp = true;

    /// <inheritdoc />
    public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        var imgDefinition = stub.Response?.Image;
        if (imgDefinition == null || imgDefinition.Type == ResponseImageType.NotSet)
        {
            return IsNotExecuted(GetType().Name);
        }

        var stubImage = stub.Response.Image;
        response.Headers.AddOrReplaceCaseInsensitive(HeaderKeys.ContentType, stubImage.ContentTypeHeaderValue);

        var cacheFilePath = Path.Combine(fileService.GetTempPath(), $"{stubImage.Hash}.bin");
        var bytes = await fileService.FileExistsAsync(cacheFilePath, cancellationToken) && !envService.IsDevelopment()
            ? await fileService.ReadAllBytesAsync(cacheFilePath, cancellationToken)
            : await GetImageAsync(stubImage, cacheFilePath, cancellationToken);
        response.Body = bytes;
        response.BodyIsBinary = bytes.Length != 0;
        return IsExecuted(GetType().Name);
    }

    /// <inheritdoc />
    public int Priority => -11;

    private Task<byte[]> GetImageAsync(
        StubResponseImageModel stubImage,
        string cacheFilePath,
        CancellationToken cancellationToken)
    {
        return UseImageSharp
            ? GetImageSharpImageAsync(stubImage, cacheFilePath, cancellationToken)
#pragma warning disable CS0162 // Unreachable code detected
            : GetImageMagickImageAsync(stubImage, cacheFilePath, cancellationToken);
#pragma warning restore CS0162 // Unreachable code detected
    }

    private Task<byte[]> GetImageMagickImageAsync(
        StubResponseImageModel stubImage,
        string cacheFilePath,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task<byte[]> GetImageSharpImageAsync(
        StubResponseImageModel stubImage,
        string cacheFilePath,
        CancellationToken cancellationToken)
    {
        var collection = new FontCollection();
        collection.Add(Path.Combine(assemblyService.GetExecutingAssemblyRootPath(),
            "Resources", "Manrope-Regular.ttf"));
        const string fontFamilyName = "Manrope";
        if (!collection.TryGet(fontFamilyName, out var family))
        {
            throw new InvalidOperationException($"Font family '{fontFamilyName}' not found!");
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

        var result = ms.ToArray();
        await fileService.WriteAllBytesAsync(cacheFilePath, result, cancellationToken);
        return result;
    }
}
