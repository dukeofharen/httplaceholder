namespace HttPlaceholder.Persistence.Db.Implementations;

/// <inheritdoc />
internal class DatabaseContextFactory : IDatabaseContextFactory
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DatabaseContextFactory(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    /// <inheritdoc />
    public IDatabaseContext CreateDatabaseContext() => new DatabaseContext(_dbConnectionFactory.GetConnection());
}
