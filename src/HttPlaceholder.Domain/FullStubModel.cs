using YamlDotNet.Serialization;

namespace HttPlaceholder.Domain
{
    /// <summary>
    /// A class for storing a stub with its metadata.
    /// </summary>
    public class FullStubModel
    {
        /// <summary>
        /// Gets or sets the stub.
        /// </summary>
        [YamlMember(Alias = "stub")]
        public StubModel Stub { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        [YamlMember(Alias = "metadata")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public StubMetadataModel Metadata { get; set; }
    }
}
