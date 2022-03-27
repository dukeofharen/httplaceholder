using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HttPlaceholder.Persistence.Db;

/// <summary>
/// Describes a class that is used to query a database.
/// </summary>
public interface IDatabaseContext : IDisposable
{
    /// <summary>
    /// Executes a query.
    /// </summary>
    /// <param name="sql">The SQL query.</param>
    /// <param name="param">A dynamic object that contains the variables for the query.</param>
    /// <returns>The number of records that are updated.</returns>
    Task<int> ExecuteAsync(string sql, object param = null);

    /// <summary>
    /// Executes a query.
    /// </summary>
    /// <param name="sql">The SQL query.</param>
    /// <param name="param">A dynamic object that contains the variables for the query.</param>
    /// <returns>The number of records that are updated.</returns>
    int Execute(string sql, object param = null);

    /// <summary>
    /// Executes a query and return the first record that was found, or null if nothing was found.
    /// </summary>
    /// <param name="sql">The SQL query.</param>
    /// <param name="param">A dynamic object that contains the variables for the query.</param>
    /// <typeparam name="TResult">The type the result should deserialize to.</typeparam>
    /// <returns>The result.</returns>
    Task<TResult> QueryFirstOrDefaultAsync<TResult>(string sql, object param = null);

    /// <summary>
    /// Executes a query and returns the list of results
    /// </summary>
    /// <param name="sql">The SQL query.</param>
    /// <param name="param">A dynamic object that contains the variables for the query.</param>
    /// <typeparam name="TResult">The type the result should deserialize to.</typeparam>
    /// <returns>A list of results.</returns>
    Task<IEnumerable<TResult>> QueryAsync<TResult>(string sql, object param = null);

    /// <summary>
    /// Executes a query and returns the list of results
    /// </summary>
    /// <param name="sql">The SQL query.</param>
    /// <param name="param">A dynamic object that contains the variables for the query.</param>
    /// <typeparam name="TResult">The type the result should deserialize to.</typeparam>
    /// <returns>A list of results.</returns>
    IEnumerable<TResult> Query<TResult>(string sql, object param = null);

    /// <summary>
    /// Execute parameterized SQL that selects a single value.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="param">The parameters to use for this command.</param>
    /// <returns>The first cell returned, as <typeparamref name="T"/>.</returns>
    Task<T> ExecuteScalarAsync<T>(string sql, object param = null);
}
