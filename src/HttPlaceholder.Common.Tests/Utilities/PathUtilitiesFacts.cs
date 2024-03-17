using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Common.Tests.Utilities;

[TestClass]
public class PathUtilitiesFacts
{
    [DataTestMethod]
    [DataRow("Folder/File.png", "Folder/File.png")]
    [DataRow("Folder\\File.png", "Folder/File.png")]
    [DataRow("", "")]
    [DataRow(null, null)]
    [DataRow("Folder/../File.png", "Folder/File.png")]
    [DataRow("Folder\\..\\File.png", "Folder/File.png")]
    [DataRow("Folder/$/File.png", "Folder/File.png")]
    [DataRow("Folder\\$\\File.png", "Folder/File.png")]
    [DataRow("Folder/?/File.png", "Folder/File.png")]
    [DataRow("Folder\\?\\File.png", "Folder/File.png")]
    [DataRow("..\\Folder\\?\\File.png", "Folder/File.png")]
    public void CleanPath_HappyFlow(string input, string expectedOutput)
    {
        // Act
        var result = PathUtilities.CleanPath(input);

        // Assert
        if (string.IsNullOrWhiteSpace(input))
        {
            Assert.AreEqual(expectedOutput, input);
        }
        else
        {
            var resultParts = SplitPath(result);
            var expectedOutputParts = SplitPath(expectedOutput);
            Assert.IsTrue(resultParts.SequenceEqual(expectedOutputParts),
                $"Expected output is {expectedOutput} but actual output is {result}.");
        }
    }

    private static IEnumerable<string> SplitPath(string path) =>
        path.Split(["/", "\\"], StringSplitOptions.None);
}
