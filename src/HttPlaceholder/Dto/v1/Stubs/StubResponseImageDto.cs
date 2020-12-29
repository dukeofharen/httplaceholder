using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Dto.v1.Stubs
{
    /// <summary>
    /// A model for storing settings about the stub image that should be generated.
    /// </summary>
    public class StubResponseImageDto : IMapFrom<StubResponseImageModel>, IMapTo<StubResponseImageModel>
    {
        /// <summary>
        /// Gets or sets the image type. Possibilities: jpeg, png, bmp and gif.
        /// </summary>
        [YamlMember(Alias = "type")]
        public string Type { get; set; } = "png";

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
        /// Gets or sets the image quality in the case of JPEG image.
        /// </summary>
        [YamlMember(Alias = "jpegQuality")]
        public int JpegQuality { get; set; } = 95;

        /// <summary>
        /// Gets or sets whether the text should be wrapped across the image or not.
        /// </summary>
        [YamlMember(Alias = "wordWrap")]
        public bool WordWrap { get; set; }
    }
}
