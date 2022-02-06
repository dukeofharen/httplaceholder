namespace HttPlaceholder.Dto.v1.ScheduledJobs;

/// <summary>
/// A model that is returned when a scheduled job has executed through the API.
/// </summary>
public class JobExecutionResultModel
{
    /// <summary>
    /// Constructs a <see cref="JobExecutionResultModel"/> instance.
    /// </summary>
    public JobExecutionResultModel(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Gets or sets message.
    /// </summary>
    public string Message { get; }
}
