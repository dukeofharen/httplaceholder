namespace HttPlaceholder.Persistence.Db
{
    /// <summary>
    /// Describes a class which purpose it is to create instances of <see cref="IDatabaseContext"/>.
    /// </summary>
    public interface IDatabaseContextFactory
    {
        IDatabaseContext CreateDatabaseContext();
    }
}
