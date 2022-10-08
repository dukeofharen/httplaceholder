using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HttPlaceholder.Common;
using HttPlaceholder.Persistence.Db;
using HttPlaceholder.Persistence.Db.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Persistence.Tests.Implementations;

[TestClass]
public class RelationalDbMigratorFacts
{
    private const string ExecutingAssemblyRootPath = "/httplaceholder";
    private readonly Mock<IDatabaseContext> _mockDatabaseContext = new();
    private readonly AutoMocker _mocker = new();

    [TestInitialize]
    public void Initialize()
    {
        var mockAssemblyService = _mocker.GetMock<IAssemblyService>();
        mockAssemblyService
            .Setup(m => m.GetExecutingAssemblyRootPath())
            .Returns(ExecutingAssemblyRootPath);
    }

    [TestMethod]
    public async Task MigrateAsync_CheckFileNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var expectedRootFolder = Path.Combine(ExecutingAssemblyRootPath, "SqlScripts/Migrations/mysql");
        SetupConfig(MysqlDbConnectionFactory.ConnectionStringKey);
        SetupMigrations(expectedRootFolder,
            new[] {new TestMigrationInput {Key = "001_initial", CheckFileFound = false, CheckResult = 0}});

        var migrator = _mocker.CreateInstance<RelationalDbMigrator>();

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                migrator.MigrateAsync(_mockDatabaseContext.Object));

        // Assert
        Assert.IsTrue(exception.Message.Contains("Could not find file"));
    }

    [TestMethod]
    public async Task MigrateAsync_HappyFlow()
    {
        // Arrange
        var expectedRootFolder = Path.Combine(ExecutingAssemblyRootPath, "SqlScripts/Migrations/mysql");
        SetupConfig(MysqlDbConnectionFactory.ConnectionStringKey);
        SetupMigrations(expectedRootFolder,
            new[]
            {
                new TestMigrationInput {Key = "001_initial", CheckFileFound = true, CheckResult = 1},
                new TestMigrationInput {Key = "002_new_table", CheckFileFound = true, CheckResult = 0}
            });

        var migrator = _mocker.CreateInstance<RelationalDbMigrator>();

        // Act
        await migrator.MigrateAsync(_mockDatabaseContext.Object);

        // Assert
        _mockDatabaseContext.Verify(m => m.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }

    [DataTestMethod]
    [DataRow(MysqlDbConnectionFactory.ConnectionStringKey, "mysql")]
    [DataRow(SqliteDbConnectionFactory.ConnectionStringKey, "sqlite")]
    [DataRow(SqlServerDbConnectionFactory.ConnectionStringKey, "mssql")]
    public void GetDatabaseMigrationsFolder_HappyFlow(string connectionStringKey, string expectedResult)
    {
        // Arrange
        SetupConfig(connectionStringKey);
        var migrator = _mocker.CreateInstance<RelationalDbMigrator>();

        // Act
        var result = migrator.GetDatabaseMigrationsFolder();

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void GetDatabaseMigrationsFolder_ConnectionStringNotSet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        SetupConfig("unknown");
        var migrator = _mocker.CreateInstance<RelationalDbMigrator>();

        // Act
        var exception =
            Assert.ThrowsException<InvalidOperationException>(() =>
                migrator.GetDatabaseMigrationsFolder());

        // Assert
        Assert.AreEqual("Could not determine migrations folder for relational DB.", exception.Message);
    }

    private void SetupMigrations(string rootFolder, IEnumerable<TestMigrationInput> inputs)
    {
        var mockFileService = _mocker.GetMock<IFileService>();
        var filesList = new List<string>();
        foreach (var input in inputs)
        {
            var migrationFilePath = Path.Join(rootFolder, $"{input.Key}.migration.sql");
            filesList.Add(migrationFilePath);
            var checkFilePath = Path.Join(rootFolder, $"{input.Key}.check.sql");
            mockFileService
                .Setup(m => m.FileExistsAsync(checkFilePath))
                .ReturnsAsync(input.CheckFileFound);
            if (input.CheckFileFound)
            {
                var checkScript = Guid.NewGuid().ToString();
                mockFileService
                    .Setup(m => m.ReadAllTextAsync(checkFilePath))
                    .ReturnsAsync(checkScript);
                _mockDatabaseContext
                    .Setup(m => m.ExecuteScalarAsync<int>(checkScript, It.IsAny<object>()))
                    .ReturnsAsync(input.CheckResult);
                if (input.CheckResult == 0)
                {
                    var migrationScript = Guid.NewGuid().ToString();
                    mockFileService
                        .Setup(m => m.ReadAllTextAsync(migrationFilePath))
                        .ReturnsAsync(migrationScript);
                    _mockDatabaseContext.Setup(m => m.ExecuteAsync(migrationScript, It.IsAny<object>()));
                }
            }
        }

        mockFileService
            .Setup(m => m.GetFiles(rootFolder, "*.migration.sql"))
            .Returns(filesList.ToArray);
    }

    private void SetupConfig(string connectionStringKey)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            {$"ConnectionStrings:{connectionStringKey}", "connection string"}
        }).Build();
        _mocker.Use<IConfiguration>(config);
    }

    private class TestMigrationInput
    {
        public string Key { get; set; }

        public bool CheckFileFound { get; set; }

        public int CheckResult { get; set; }
    }
}
