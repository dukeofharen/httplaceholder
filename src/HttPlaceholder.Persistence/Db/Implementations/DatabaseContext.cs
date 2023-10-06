using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <inheritdoc />
internal class DatabaseContext : IDatabaseContext
{
    internal readonly DbDataSource DataSource;
    internal readonly IDbConnection DbConnection;

    public DatabaseContext(IDbConnection dbConnection)
    {
        DbConnection = dbConnection;
        dbConnection.Open();
    }

    public DatabaseContext(DbDataSource dataSource) : this(dataSource.CreateConnection())
    {
        DataSource = dataSource;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        DbConnection?.Dispose();
        DataSource?.Dispose();
    }

    /// <inheritdoc />
    public async Task<int> ExecuteAsync(string sql, CancellationToken cancellationToken, object param = null) =>
        await DbConnection.ExecuteAsync(new CommandDefinition(sql, param, cancellationToken: cancellationToken));

    /// <inheritdoc />
    public int Execute(string sql, object param = null) => DbConnection.Execute(sql, param);

    /// <inheritdoc />
    public async Task<TResult> QueryFirstOrDefaultAsync<TResult>(string sql, CancellationToken cancellationToken,
        object param = null) =>
        await DbConnection.QueryFirstOrDefaultAsync<TResult>(new CommandDefinition(sql, param,
            cancellationToken: cancellationToken));

    /// <inheritdoc />
    public async Task<IEnumerable<TResult>> QueryAsync<TResult>(string sql, CancellationToken cancellationToken,
        object param = null) =>
        await DbConnection.QueryAsync<TResult>(new CommandDefinition(sql, param,
            cancellationToken: cancellationToken));

    /// <inheritdoc />
    public IEnumerable<TResult> Query<TResult>(string sql, object param = null) =>
        DbConnection.Query<TResult>(sql, param);

    /// <inheritdoc />
    public Task<T> ExecuteScalarAsync<T>(string sql, CancellationToken cancellationToken, object param = null) =>
        DbConnection.ExecuteScalarAsync<T>(new CommandDefinition(sql, param, cancellationToken: cancellationToken));
}
