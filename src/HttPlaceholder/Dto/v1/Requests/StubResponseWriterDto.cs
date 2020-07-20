using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

namespace HttPlaceholder.Dto.v1.Requests
{
    /// <summary>
    /// A model for storing the execution result of a specific response writer.
    /// </summary>
    public class StubResponseWriterResultDto : IMapFrom<StubResponseWriterResultModel>, IMapTo<StubResponseWriterResultModel>
    {
        /// <summary>
        /// Gets or sets the name of the response writer.
        /// </summary>
        public string ResponseWriterName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="StubResponseWriterResultDto"/> is executed.
        /// </summary>
        public bool Executed { get; set; }
    }
}
