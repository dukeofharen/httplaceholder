using System.Data;
using System.Data.Common;
using HttPlaceholder.Persistence.Db;
using HttPlaceholder.Persistence.Db.Implementations;

namespace HttPlaceholder.Persistence.Tests.Db;

[TestClass]
public class DatabaseContextFactoryFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void CreateDatabaseContext_NoDataSourceOrConnection_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var factory = _mocker.CreateInstance<DatabaseContextFactory>();
        var dbConnectionFactoryMock = _mocker.GetMock<IDbConnectionFactory>();
        dbConnectionFactoryMock
            .Setup(m => m.GetDataSource())
            .Returns((DbDataSource)null);
        dbConnectionFactoryMock
            .Setup(m => m.GetConnection())
            .Returns((IDbConnection)null);

        // Act
        var exception = Assert.ThrowsException<InvalidOperationException>(() => factory.CreateDatabaseContext());

        // Assert
        Assert.IsTrue(
            exception.Message.Contains("No data source and no connection set for DB connection factory of type"));
    }

    [TestMethod]
    public void CreateDatabaseContext_ConnectionIsSet_ShouldReturnDbContextWithConnection()
    {
        // Arrange
        var factory = _mocker.CreateInstance<DatabaseContextFactory>();
        var dbConnectionFactoryMock = _mocker.GetMock<IDbConnectionFactory>();
        dbConnectionFactoryMock
            .Setup(m => m.GetDataSource())
            .Returns((DbDataSource)null);

        var connectionMock = new Mock<IDbConnection>();
        dbConnectionFactoryMock
            .Setup(m => m.GetConnection())
            .Returns(connectionMock.Object);

        // Act
        var result = (DatabaseContext)factory.CreateDatabaseContext();

        // Assert
        Assert.AreEqual(connectionMock.Object, result.DbConnection);
        Assert.IsNull(result.DataSource);
        connectionMock.Verify(m => m.Open());
    }

    [TestMethod]
    public void CreateDatabaseContext_DataSourceIsSet_ShouldReturnDbContextWithDataSource()
    {
        // Arrange
        var factory = _mocker.CreateInstance<DatabaseContextFactory>();
        var dbConnectionFactoryMock = _mocker.GetMock<IDbConnectionFactory>();

        var dbConnectionMock = new Mock<DbConnection>();
        var dataSource = new TestDataSource(dbConnectionMock.Object);
        dbConnectionFactoryMock
            .Setup(m => m.GetDataSource())
            .Returns(dataSource);

        // Act
        var result = (DatabaseContext)factory.CreateDatabaseContext();

        // Assert
        Assert.AreEqual(dbConnectionMock.Object, result.DbConnection);
        Assert.AreEqual(dataSource, result.DataSource);
    }

    private class TestDataSource : DbDataSource
    {
        private readonly DbConnection _dbConnection;

        public TestDataSource(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public override string ConnectionString { get; }

        protected override DbConnection CreateDbConnection() => _dbConnection;
    }
}
