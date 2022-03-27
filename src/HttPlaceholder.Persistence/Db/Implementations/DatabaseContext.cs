using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <inheritdoc />
internal class DatabaseContext : IDatabaseContext
{
    private readonly IDbConnection _dbConnection;

    public DatabaseContext(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    /// <inheritdoc />
    public void Dispose() => _dbConnection?.Dispose();

    /// <inheritdoc />
    public async Task<int> ExecuteAsync(string sql, object param = null) =>
        await _dbConnection.ExecuteAsync(sql, param);

    /// <inheritdoc />
    public int Execute(string sql, object param = null) => _dbConnection.Execute(sql, param);

    /// <inheritdoc />
    public async Task<TResult> QueryFirstOrDefaultAsync<TResult>(string sql, object param = null) =>
        await _dbConnection.QueryFirstOrDefaultAsync<TResult>(sql, param);

    /// <inheritdoc />
    public async Task<IEnumerable<TResult>> QueryAsync<TResult>(string sql, object param = null) =>
        await _dbConnection.QueryAsync<TResult>(sql, param);

    /// <inheritdoc />
    public IEnumerable<TResult> Query<TResult>(string sql, object param = null) =>
        _dbConnection.Query<TResult>(sql, param);

    /// <inheritdoc />
    public Task<T> ExecuteScalarAsync<T>(string sql, object param = null) =>
        _dbConnection.ExecuteScalarAsync<T>(sql, param);
}
