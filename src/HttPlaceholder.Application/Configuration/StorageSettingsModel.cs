namespace HttPlaceholder.Application.Configuration;

/// <summary>
/// A model for storing storage settings.
/// </summary>
public class StorageSettingsModel
{
    /// <summary>
    /// Gets or sets the stub input file location.
    /// </summary>
    public string InputFile { get; set; }

    /// <summary>
    /// Gets or sets the length the old request queue may be.
    /// </summary>
    public int OldRequestsQueueLength { get; set; } = 40;

    /// <summary>
    /// Gets or sets whether the request should be logged.
    /// </summary>
    public bool EnableRequestLogging { get; set; }

    /// <summary>
    /// Gets or sets the file storage location.
    /// </summary>
    public string FileStorageLocation { get; set; }

    /// <summary>
    /// Gets or sets whether everything should be stored in memory.
    /// </summary>
    public bool UseInMemoryStorage { get; set; }

    /// <summary>
    /// Gets or sets whether the deletion of old requests should be done in a background job.
    /// </summary>
    public bool CleanOldRequestsInBackgroundJob { get; set; }
}
