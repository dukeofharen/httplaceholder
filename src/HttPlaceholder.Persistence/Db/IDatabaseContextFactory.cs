namespace HttPlaceholder.Persistence.Db;

/// <summary>
///     Describes a class which purpose it is to create instances of <see cref="IDatabaseContext" />.
/// </summary>
public interface IDatabaseContextFactory
{
    /// <summary>
    ///     Creates a <see cref="IDatabaseContext" /> instance.
    /// </summary>
    /// <returns>A <see cref="IDatabaseContext" /> instance.</returns>
    IDatabaseContext CreateDatabaseContext();
}
