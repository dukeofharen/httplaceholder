using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using ImageMagick;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using TextAlignment = ImageMagick.TextAlignment;

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
    private bool UseImageSharp = false;

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
        var bytes =
            response.Body = await GetImageAsync(stubImage, cacheFilePath, cancellationToken);
        response.BodyIsBinary = bytes.Length != 0;
        return StubResponseWriterResultModel.IsExecuted(GetType().Name);
    }

    /// <inheritdoc />
    public int Priority => -11;

    private async Task<byte[]> GetImageAsync(
        StubResponseImageModel stubImage,
        string cacheFilePath,
        CancellationToken cancellationToken)
    {
        if (await fileService.FileExistsAsync(cacheFilePath, cancellationToken) && !envService.IsDevelopment())
        {
            return await fileService.ReadAllBytesAsync(cacheFilePath, cancellationToken);
        }

        var result = UseImageSharp
            ? await GetImageSharpImageAsync(stubImage, cancellationToken)
            : await GetImageMagickImageAsync(stubImage, cancellationToken);
        await fileService.WriteAllBytesAsync(cacheFilePath, result, cancellationToken);
        return result;
    }

    private Task<byte[]> GetImageMagickImageAsync(
        StubResponseImageModel stubImage,
        CancellationToken cancellationToken)
    {
        var backgroundColor = new MagickColor(stubImage.BackgroundColor);
        using var image = new MagickImage(backgroundColor, stubImage.Width, stubImage.Height);
        var fontColor = !string.IsNullOrWhiteSpace(stubImage.FontColor)
            ? new MagickColor(stubImage.FontColor)
            : new MagickColor("#000000"); // TODO invert color
        const int fontSize = 30;
        if (stubImage.WordWrap)
        {
            using var label = new MagickImage($"caption:{stubImage.Text}",
                new MagickReadSettings
                {
                    FillColor = fontColor,
                    BackgroundColor = MagickColors.Transparent,
                    FontPointsize = fontSize,
                    Width = stubImage.Width,
                    TextGravity = Gravity.Center,
                    Font = GetFontPath()
                });
            image.Composite(label, 0, (stubImage.Height - label.Height) / 2, CompositeOperator.Over);
        }
        else
        {
            new Drawables()
                .FontPointSize(fontSize)
                .FillColor(fontColor)
                .TextAlignment(TextAlignment.Center)
                .Text(stubImage.Width / 2, stubImage.Height / 2, stubImage.Text)
                .Font(GetFontPath())
                .Draw(image);
        }

        switch (stubImage.Type)
        {
            case ResponseImageType.Bmp:
                image.Format = MagickFormat.Bmp;
                break;
            case ResponseImageType.Gif:
                image.Format = MagickFormat.Gif;
                break;
            case ResponseImageType.Jpeg:
                image.Format = MagickFormat.Jpeg;
                image.Quality = stubImage.JpegQuality;
                break;
            default:
                image.Format = MagickFormat.Png;
                break;
        }

        return Task.FromResult(image.ToByteArray());
    }

    private async Task<byte[]> GetImageSharpImageAsync(
        StubResponseImageModel stubImage,
        CancellationToken cancellationToken)
    {
        var collection = new FontCollection();
        collection.Add(GetFontPath());
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
        return result;
    }

    private string GetFontPath() =>
        Path.Combine(assemblyService.GetExecutingAssemblyRootPath(),
            "Resources", "Manrope-Regular.ttf");
}
