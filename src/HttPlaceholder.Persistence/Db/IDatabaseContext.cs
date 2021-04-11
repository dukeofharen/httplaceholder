using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HttPlaceholder.Persistence.Db
{
    /// <summary>
    /// Describes a class that is used to query a database.
    /// </summary>
    public interface IDatabaseContext : IDisposable
    {
        Task<int> ExecuteAsync(string sql, object param = null);

        int Execute(string sql, object param = null);

        Task<TResult> QueryFirstOrDefaultAsync<TResult>(string sql, object param = null);

        Task<IEnumerable<TResult>> QueryAsync<TResult>(string sql, object param = null);

        IEnumerable<TResult> Query<TResult>(string sql, object param = null);
    }
}
