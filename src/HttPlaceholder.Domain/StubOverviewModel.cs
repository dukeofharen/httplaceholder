namespace HttPlaceholder.Domain;

/// <summary>
///     A class for storing a stripped down version of a stub.
/// </summary>
public class StubOverviewModel
{
    /// <summary>
    ///     Gets or sets the identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     Gets or sets the tenant.
    /// </summary>
    public string Tenant { get; set; }

    /// <summary>
    ///     Gets or sets whether the stub is enabled or not.
    /// </summary>
    public bool Enabled { get; set; }
}
