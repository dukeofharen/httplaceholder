namespace HttPlaceholder.Client.Dto.ScheduledJobs;

/// <summary>
///     A model that is returned when a scheduled job has executed through the API.
/// </summary>
public class JobExecutionResultDto
{
    /// <summary>
    ///     Constructs a <see cref="JobExecutionResultDto" /> instance.
    /// </summary>
    public JobExecutionResultDto(string message)
    {
        Message = message;
    }

    /// <summary>
    ///     Gets or sets message.
    /// </summary>
    public string Message { get; }
}
