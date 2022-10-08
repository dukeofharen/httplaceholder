using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <summary>
/// A class that is used to perform database migrations.
/// </summary>
internal class RelationalDbMigrator : IRelationalDbMigrator
{
    private readonly IConfiguration _configuration;
    private readonly IFileService _fileService;
    private readonly IAssemblyService _assemblyService;
    private readonly ILogger<RelationalDbMigrator> _logger;

    public RelationalDbMigrator(IConfiguration configuration, IFileService fileService,
        IAssemblyService assemblyService, ILogger<RelationalDbMigrator> logger)
    {
        _configuration = configuration;
        _fileService = fileService;
        _assemblyService = assemblyService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task MigrateAsync(IDatabaseContext ctx)
    {
        var dbFolder = GetDatabaseMigrationsFolder();
        var migrationsRootFolder = Path.Combine(_assemblyService.GetExecutingAssemblyRootPath(),
            "SqlScripts/Migrations", dbFolder);
        var migrationFiles = (await _fileService.GetFilesAsync(migrationsRootFolder, "*.migration.sql")).OrderBy(f => f);
        foreach (var file in migrationFiles)
        {
            var fileBaseName = Path.GetFileName(file).Split('.').First();
            var checkFileName = $"{fileBaseName}.check.sql";
            var checkFilePath = Path.Combine(migrationsRootFolder, checkFileName);
            if (!await _fileService.FileExistsAsync(checkFilePath))
            {
                throw new InvalidOperationException($"Could not find file {checkFilePath}");
            }

            // The check script will be loaded. It is expected that the script returns an 1 or higher if the migration should NOT be executed and a "0" if the migration SHOULD be executed.
            var checkScript = await _fileService.ReadAllTextAsync(checkFilePath);
            _logger.LogDebug($"Checking file {checkFileName}.");
            var checkResult = await ctx.ExecuteScalarAsync<int>(checkScript);
            if (checkResult > 0)
            {
                _logger.LogDebug($"Result of {checkFileName} is {checkResult}, so migration will not be executed.");
            }
            else
            {
                _logger.LogDebug($"Result of {checkFileName} is {checkResult}, so migration {file} will be executed.");
                var migrationScript = await _fileService.ReadAllTextAsync(file);
                await ctx.ExecuteAsync(migrationScript);
            }
        }
    }

    internal string GetDatabaseMigrationsFolder()
    {
        if (!string.IsNullOrWhiteSpace(
                _configuration.GetConnectionString(MysqlDbConnectionFactory.ConnectionStringKey)))
        {
            return "mysql";
        }

        if (!string.IsNullOrWhiteSpace(
                _configuration.GetConnectionString(SqliteDbConnectionFactory.ConnectionStringKey)))
        {
            return "sqlite";
        }

        if (!string.IsNullOrWhiteSpace(
                _configuration.GetConnectionString(SqlServerDbConnectionFactory.ConnectionStringKey)))
        {
            return "mssql";
        }

        throw new InvalidOperationException("Could not determine migrations folder for relational DB.");
    }
}
