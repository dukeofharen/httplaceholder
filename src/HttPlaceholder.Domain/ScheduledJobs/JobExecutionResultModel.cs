namespace HttPlaceholder.Domain.ScheduledJobs;

/// <summary>
///     A model that is returned when a scheduled job has been executed.
/// </summary>
public class JobExecutionResultModel
{
    /// <summary>
    ///     Constructs a <see cref="JobExecutionResultModel" /> instance.
    /// </summary>
    public JobExecutionResultModel(string message, bool failed)
    {
        Message = message;
        Failed = failed;
    }

    /// <summary>
    ///     Gets or sets message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    ///     Gets or sets whether the job has failed.
    /// </summary>
    public bool Failed { get; }
}
