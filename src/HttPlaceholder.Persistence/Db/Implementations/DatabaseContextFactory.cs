namespace HttPlaceholder.Persistence.Db.Implementations;

internal class DatabaseContextFactory : IDatabaseContextFactory
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DatabaseContextFactory(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public IDatabaseContext CreateDatabaseContext() => new DatabaseContext(_dbConnectionFactory.GetConnection());
}