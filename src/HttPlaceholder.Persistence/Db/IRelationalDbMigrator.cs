using System.Threading.Tasks;

namespace HttPlaceholder.Persistence.Db;

/// <summary>
/// Describes a class that is used to perform database migrations.
/// </summary>
public interface IRelationalDbMigrator
{
    /// <summary>
    /// Migrates the currently configured stub database to the latest version.
    /// </summary>
    /// <param name="ctx">The database context.</param>
    Task MigrateAsync(IDatabaseContext ctx);
}
