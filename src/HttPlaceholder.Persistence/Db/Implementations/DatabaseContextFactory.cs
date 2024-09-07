using System;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <inheritdoc />
internal class DatabaseContextFactory(IDbConnectionFactory dbConnectionFactory) : IDatabaseContextFactory
{
    /// <inheritdoc />
    public IDatabaseContext CreateDatabaseContext()
    {
        var dataSource = dbConnectionFactory.GetDataSource();
        if (dataSource != null)
        {
            return new DatabaseContext(dataSource);
        }

        var connection = dbConnectionFactory.GetConnection();
        if (connection != null)
        {
            return new DatabaseContext(connection);
        }

        throw new InvalidOperationException(string.Format(PersistenceResources.NoDataSourceOrConnection,
            dbConnectionFactory.GetType()));
    }
}
