using System.Collections.Generic;
using HttPlaceholder.Client.Dto.Requests;

namespace HttPlaceholder.Client.Verification.Dto;

/// <summary>
/// A model that contains the stub verification results.
/// </summary>
public class VerificationResultModel
{
    /// <summary>
    /// Gets or sets whether the verification passed.
    /// </summary>
    public bool Passed { get; set; }

    /// <summary>
    /// Gets or sets the validation message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the requests.
    /// </summary>
    public IEnumerable<RequestResultDto> Requests { get; set; }
}
