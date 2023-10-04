using System;

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
    public IDatabaseContext CreateDatabaseContext()
    {
        var dataSource = _dbConnectionFactory.GetDataSource();
        if (dataSource != null)
        {
            return new DatabaseContext(dataSource);
        }

        var connection = _dbConnectionFactory.GetConnection();
        if (connection != null)
        {
            return new DatabaseContext(connection);
        }

        throw new InvalidOperationException(
            $"No data source and no connection set for DB connection factory of type {_dbConnectionFactory.GetType()}.");
    }
}
