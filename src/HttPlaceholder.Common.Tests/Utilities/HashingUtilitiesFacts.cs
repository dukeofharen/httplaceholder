using HttPlaceholder.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class HashingUtilitiesFacts
{
    [TestMethod]
    public void GetMd5String_HappyFlow()
    {
        // Arrange
        const string input = "test 123";
        const string expectedOutput = "39d0d586a701e199389d954f2d592720";

        // Act
        var result = HashingUtilities.GetMd5String(input);

        // Assert
        Assert.AreEqual(expectedOutput, result);
    }

    [TestMethod]
    public void GetSha512String_HappyFlow()
    {
        // Arrange
        const string input = "test 123";
        const string expectedOutput = "RNiw9ZCfWvIt1SyjF8exd7hnDQHw8KK1iVcbqV+fyPt6yij3RnZkJS1SsuEmtxH4jjllJO4f3HK3Rjp0hKIcbw==";

        // Act
        var result = HashingUtilities.GetSha512String(input);

        // Assert
        Assert.AreEqual(expectedOutput, result);
    }
}
