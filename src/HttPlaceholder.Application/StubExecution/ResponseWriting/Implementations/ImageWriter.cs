﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class ImageWriter : IResponseWriter
    {
        private readonly IAssemblyService _assemblyService;

        public ImageWriter(IAssemblyService assemblyService)
        {
            _assemblyService = assemblyService;
        }

        public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (stub.Response?.Image == null)
            {
                return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
            }

            var stubImage = stub.Response.Image;
            var type = stubImage.Type;
            if (!Constants.AllowedImageTypes.Any(t =>
                string.Equals(t, type, StringComparison.OrdinalIgnoreCase)))
            {
                return StubResponseWriterResultModel.IsExecuted(GetType().Name,
                    $"Type '{type}' not allowed for stub image generation. Possibilities: {string.Join(", ", Constants.AllowedImageTypes)}");
            }

            var collection = new FontCollection();
            collection.Install(Path.Combine(_assemblyService.GetExecutingAssemblyRootPath(), "Manrope-Regular.ttf"));
            var fontFamilyName = "Manrope";
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
                i.Fill(parsedColor, polygon).ApplyScalingWaterMark(font, stubImage.Text, fontColor, 5,
                    stubImage.WordWrap));
            using var ms = new MemoryStream();
            string contentType;
            switch (stubImage.Type)
            {
                case Constants.BmpType:
                    await image.SaveAsBmpAsync(ms);
                    contentType = "image/bmp";
                    break;
                case Constants.GifType:
                    await image.SaveAsGifAsync(ms);
                    contentType = "image/gif";
                    break;
                case Constants.JpegType:
                    await image.SaveAsJpegAsync(ms, new JpegEncoder {Quality = stubImage.JpegQuality});
                    contentType = "image/jpeg";
                    break;
                default:
                    await image.SaveAsPngAsync(ms);
                    contentType = "image/png";
                    break;
            }

            response.Body = ms.ToArray();
            response.Headers.AddOrReplaceCaseInsensitive("Content-Type", contentType);
            return StubResponseWriterResultModel.IsExecuted(GetType().Name);
        }

        public int Priority { get; } = 0;
    }
}
