using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <summary>
///     A class that is used to perform database migrations.
/// </summary>
internal class RelationalDbMigrator(
    IConfiguration configuration,
    IFileService fileService,
    IAssemblyService assemblyService,
    ILogger<RelationalDbMigrator> logger)
    : IRelationalDbMigrator
{
    /// <inheritdoc />
    public async Task MigrateAsync(IDatabaseContext ctx, CancellationToken cancellationToken)
    {
        var dbFolder = GetDatabaseMigrationsFolder();
        var migrationsRootFolder = Path.Combine(assemblyService.GetExecutingAssemblyRootPath(),
            "SqlScripts/Migrations", dbFolder);
        var migrationFiles =
            (await fileService.GetFilesAsync(migrationsRootFolder, "*.migration.sql", cancellationToken)).OrderBy(f =>
                f);
        foreach (var file in migrationFiles)
        {
            var fileBaseName = Path.GetFileName(file).Split('.').First();
            var checkFileName = $"{fileBaseName}.check.sql";
            var checkFilePath = Path.Combine(migrationsRootFolder, checkFileName);
            if (!await fileService.FileExistsAsync(checkFilePath, cancellationToken))
            {
                throw new InvalidOperationException($"Could not find file {checkFilePath}");
            }

            await ExecuteMigrationAsync(ctx, checkFilePath, checkFileName, file, cancellationToken);
        }
    }

    internal string GetDatabaseMigrationsFolder()
    {
        if (!string.IsNullOrWhiteSpace(
                configuration.GetConnectionString(MysqlDbConnectionFactory.ConnectionStringKey)))
        {
            return "mysql";
        }

        if (!string.IsNullOrWhiteSpace(
                configuration.GetConnectionString(SqliteDbConnectionFactory.ConnectionStringKey)))
        {
            return "sqlite";
        }

        if (!string.IsNullOrWhiteSpace(
                configuration.GetConnectionString(SqlServerDbConnectionFactory.ConnectionStringKey)))
        {
            return "mssql";
        }

        if (!string.IsNullOrWhiteSpace(
                configuration.GetConnectionString(PostgresDbConnectionFactory.ConnectionStringKey)))
        {
            return "postgres";
        }

        throw new InvalidOperationException("Could not determine migrations folder for relational DB.");
    }

    private async Task ExecuteMigrationAsync(
        IDatabaseContext ctx,
        string checkFilePath,
        string checkFileName,
        string file,
        CancellationToken cancellationToken)
    {
        // The check script will be loaded. It is expected that the script returns an 1 or higher if the migration should NOT be executed and a "0" if the migration SHOULD be executed.
        var checkScript = await fileService.ReadAllTextAsync(checkFilePath, cancellationToken);
        logger.LogDebug("Checking file {CheckFilename}.", checkFileName);
        var checkResult = await ctx.ExecuteScalarAsync<int>(checkScript, cancellationToken);
        if (checkResult > 0)
        {
            logger.LogDebug("Result of {CheckFilename} is {CheckResult}, so migration will not be executed.",
                checkFileName, checkResult);
        }
        else
        {
            logger.LogDebug(
                "Result of {CheckFilename} is {CheckResult}, so migration {MigrationFile} will be executed.",
                checkFileName, checkResult, file);
            var migrationScript = await fileService.ReadAllTextAsync(file, cancellationToken);
            await ctx.ExecuteAsync(migrationScript, cancellationToken);
        }
    }
}
