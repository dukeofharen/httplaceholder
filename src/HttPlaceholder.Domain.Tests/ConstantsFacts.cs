using System;

namespace HttPlaceholder.Domain.Tests;

[TestClass]
public class ConstantsFacts
{
    [DataTestMethod]
    [DataRow("--port 80 --verbose", "", true)]
    [DataRow("--port 80 -V", "", true)]
    [DataRow("--port 80 -v", "", false)]
    [DataRow("--port 80", "true", true)]
    [DataRow("--port 80", "TRuE", true)]
    public void CliArgs_IsVerbose(string args, string verboseEnv, bool isVerbose)
    {
        // Arrange
        var argsArray = args.Split(' ');
        Environment.SetEnvironmentVariable("verbose", verboseEnv, EnvironmentVariableTarget.Process);

        // Act
        var result = CliArgs.IsVerbose(argsArray);

        // Assert
        Assert.AreEqual(isVerbose, result);
    }
}
