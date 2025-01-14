﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using ImageMagick;
using ImageMagick.Drawing;
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

        var result = GetImageMagickImageAsync(stubImage);
        await fileService.WriteAllBytesAsync(cacheFilePath, result, cancellationToken);
        return result;
    }

    private byte[] GetImageMagickImageAsync(
        StubResponseImageModel stubImage)
    {
        var backgroundColor = new MagickColor(stubImage.BackgroundColor);
        var width = Convert.ToUInt32(stubImage.Width);
        var height = Convert.ToUInt32(stubImage.Height);
        using var image = new MagickImage(backgroundColor, width, height);
        var fontColor = !string.IsNullOrWhiteSpace(stubImage.FontColor)
            ? new MagickColor(stubImage.FontColor)
            : backgroundColor.InvertRgbColor();
        var fontSize = stubImage.FontSize > 0 ? stubImage.FontSize : 30;
        if (stubImage.WordWrap)
        {
            using var label = new MagickImage($"caption:{stubImage.Text}",
                new MagickReadSettings
                {
                    FillColor = fontColor,
                    BackgroundColor = MagickColors.Transparent,
                    FontPointsize = fontSize,
                    Width = width,
                    TextGravity = Gravity.Center,
                    Font = GetFontPath()
                });
            var bla = (int)(height - label.Height) / 2;
            image.Composite(label, Gravity.Center, (int)0, bla, CompositeOperator.Over);
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
                image.Quality = Convert.ToUInt32(stubImage.JpegQuality);
                break;
            default:
                image.Format = MagickFormat.Png;
                break;
        }

        return image.ToByteArray();
    }

    private string GetFontPath() =>
        Path.Combine(assemblyService.GetExecutingAssemblyRootPath(),
            "Resources", "Manrope-Regular.ttf");
}
