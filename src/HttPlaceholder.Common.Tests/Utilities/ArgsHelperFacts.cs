using System.Collections.Generic;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class ArgsHelperFacts
{
    [TestMethod]
    public void Parse_HappyFlow()
    {
        // Arrange
        var args = GetArgs("--var1 value1 --var2 this is value 2");

        // Act
        var result = args.Parse();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("value1", result["var1"]);
        Assert.AreEqual("this is value 2", result["var2"]);
    }

    [TestMethod]
    public void EnsureEntryExists_EntryAlreadyExists_ShouldNotAddEntry()
    {
        // Arrange
        var dict = new Dictionary<string, string> { { "var1", "value1" } };

        // Act
        dict.EnsureEntryExists("var1", "value2");

        // Assert
        Assert.AreEqual("value1", dict["var1"]);
    }

    [TestMethod]
    public void EnsureEntryExists_EntryDoesNotExist_ShouldAddEntry()
    {
        // Arrange
        var dict = new Dictionary<string, string> { { "var2", "value2" } };

        // Act
        dict.EnsureEntryExists("var1", "value1");

        // Assert
        Assert.AreEqual("value1", dict["var1"]);
    }

    private static string[] GetArgs(string input) => input.Split(' ');
}
