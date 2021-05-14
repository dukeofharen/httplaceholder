using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Metadata;

namespace HttPlaceholder.Client
{
    /// <summary>
    /// Describes a class that is used to communicate with HttPlaceholder.
    /// </summary>
    public interface IHttPlaceholderClient
    {
        /// <summary>
        /// Retrieves the HttPlaceholder metadata.
        /// </summary>
        /// <returns>The metadata.</returns>
        Task<MetadataDto> GetMetadataAsync();
    }
}
