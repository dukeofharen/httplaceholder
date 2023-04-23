namespace HttPlaceholder.Application.Import.Commands;

/// <summary>
///     An interface used to describe a class that is used to import stubs.
/// </summary>
public interface IImportStubsCommand
{
    /// <summary>
    ///     Gets the import input.
    /// </summary>
    public string Input { get; }

    /// <summary>
    ///     Gets whether to create stubs or not.
    /// </summary>
    public bool DoNotCreateStub { get; }

    /// <summary>
    ///     Gets the stub tenant.
    /// </summary>
    public string Tenant { get; }

    /// <summary>
    ///     A prefix that is put before the stub ID.
    /// </summary>
    public string StubIdPrefix { get; }
}
