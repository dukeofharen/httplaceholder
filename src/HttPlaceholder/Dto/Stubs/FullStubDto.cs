using YamlDotNet.Serialization;

namespace HttPlaceholder.Dto.Stubs
{
    /// <summary>
    /// A class for storing a stub with its metadata.
    /// </summary>
    public class FullStubDto
    {
        /// <summary>
        /// Gets or sets the stub.
        /// </summary>
        [YamlMember(Alias = "stub")]
        public StubDto Stub { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        [YamlMember(Alias = "metadata")]
        public StubMetadataDto Metadata { get; set; }
    }
}
