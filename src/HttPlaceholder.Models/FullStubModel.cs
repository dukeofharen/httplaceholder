using YamlDotNet.Serialization;

namespace HttPlaceholder.Models
{
    public class FullStubModel
    {
        [YamlMember(Alias = "stub")]
        public StubModel Stub { get; set; }

        [YamlMember(Alias = "metadata")]
        public StubMetadataModel Metadata { get; set; }
    }
}
