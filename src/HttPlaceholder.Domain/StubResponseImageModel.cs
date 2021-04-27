using System.ComponentModel.DataAnnotations;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Common.Validation;
using HttPlaceholder.Domain.Enums;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Domain
{
    /// <summary>
    /// A model for storing settings about the stub image that should be generated.
    /// </summary>
    public class StubResponseImageModel
    {
        private const string ColorRegex = "^#[A-Fa-f0-9]{6}$";

        /// <summary>
        /// Gets or sets the image type. Possibilities: jpeg, png, bmp and gif.
        /// </summary>
        [YamlMember(Alias = "type")]
        public ResponseImageType? Type { get; set; }

        /// <summary>
        /// Gets or sets the image width in pixels.
        /// </summary>
        [YamlMember(Alias = "width")]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the image height in pixels.
        /// </summary>
        [YamlMember(Alias = "height")]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the background color in HEX.
        /// </summary>
        [YamlMember(Alias = "backgroundColor")]
        [RegularExpression(ColorRegex,
            ErrorMessage = "Field 'BackgroundColor' should be filled with a valid hex color code (e.g. '#1234AF').")]
        public string BackgroundColor { get; set; } = "#3d3d3d";

        /// <summary>
        /// Gets or sets the text that should be drawn in the image.
        /// </summary>
        [YamlMember(Alias = "text")]
        public string Text { get; set; } = "HttPlaceholder";

        /// <summary>
        /// Gets or sets the fontsize for the text in the image.
        /// </summary>
        [YamlMember(Alias = "fontSize")]
        public int FontSize { get; set; } = 7;

        /// <summary>
        /// Gets or sets the font color.
        /// </summary>
        [YamlMember(Alias = "fontColor")]
        [RegularExpression(ColorRegex,
            ErrorMessage = "Field 'FontColor' should be filled with a valid hex color code (e.g. '#1234AF').")]
        public string FontColor { get; set; }

        /// <summary>
        /// Gets or sets the image quality in the case of JPEG image.
        /// </summary>
        [YamlMember(Alias = "jpegQuality")]
        [Between(1, 100, true)]
        public int JpegQuality { get; set; } = 95;

        /// <summary>
        /// Gets or sets whether the text should be wrapped across the image or not.
        /// </summary>
        [YamlMember(Alias = "wordWrap")]
        public bool WordWrap { get; set; }

        [JsonIgnore] public string Hash => HashingUtilities.GetMd5String(JsonConvert.SerializeObject(this));

        public string ContentTypeHeaderValue
        {
            get
            {
                switch (Type)
                {
                    case ResponseImageType.Bmp:
                        return "image/bmp";
                    case ResponseImageType.Gif:
                        return "image/gif";
                    case ResponseImageType.Jpeg:
                        return "image/jpeg";
                    default:
                        return "image/png";
                }
            }
        }
    }
}
