namespace HttPlaceholder.Application.StubExecution;

/// <summary>
///     Describes a class that is used to set and get stub related data for a specific HTTP request.
/// </summary>
public interface IStubRequestContext
{
    /// <summary>
    ///     Gets or sets the stub distribution key. Returns null if stubs, requests and other models should not be split.
    ///     The distribution key is used to, for example, be able to save stubs and requests for a specific user
    ///     (e.g. in case of a SaaS version of the application where multiple users' stubs are saved in the same data source).
    /// </summary>
    string DistributionKey { get; set; }
}
