using System;
using System.Threading.Tasks;

namespace HttPlaceholder.Common;

/// <summary>
/// Describes a class that is used to work with files.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Reads all bytes of a file.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>The file bytes.</returns>
    Task<byte[]> ReadAllBytesAsync(string path);

    /// <summary>
    /// Reads all text of a file.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>The file text.</returns>
    string ReadAllText(string path);

    /// <summary>
    /// Reads all text of a file.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>The file text.</returns>
    Task<string> ReadAllTextAsync(string path);

    /// <summary>
    /// Writes all bytes to a file.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="contents">The file contents.</param>
    Task WriteAllBytesAsync(string path, byte[] contents);

    /// <summary>
    /// Writes all text to a file.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="contents">The file contents.</param>
    void WriteAllText(string path, string contents);

    /// <summary>
    /// Checks whether a file exists.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>True if the file exists, false otherwise.</returns>
    bool FileExists(string path);

    /// <summary>
    /// Checks whether a directory exists.
    /// </summary>
    /// <param name="path">The directory path.</param>
    /// <returns>True if the directory exists, false otherwise.</returns>
    bool DirectoryExists(string path);

    /// <summary>
    /// Creates a directory.
    /// </summary>
    /// <param name="path">The directory path.</param>
    void CreateDirectory(string path);

    /// <summary>
    /// Returns the temporary path of this PC.
    /// </summary>
    /// <returns>The temporary path.</returns>
    string GetTempPath();

    /// <summary>
    /// Deletes a file.
    /// </summary>
    /// <param name="path">The file path.</param>
    void DeleteFile(string path);

    /// <summary>
    /// Gets the last write date and time of a specific file.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>The last write <see cref="DateTime"/>.</returns>
    DateTime GetLastWriteTime(string path);

    /// <summary>
    /// Checks whether the given path is a directory.
    /// </summary>
    /// <param name="path">The directory path.</param>
    /// <returns>True if the path is a directory, false otherwise.</returns>
    bool IsDirectory(string path);

    /// <summary>
    /// Returns a list of files.
    /// </summary>
    /// <param name="path">The directory path.</param>
    /// <param name="searchPattern">A file search pattern to limit the number of files returned.</param>
    /// <returns>An array of file names.</returns>
    string[] GetFiles(string path, string searchPattern);

    /// <summary>
    /// Returns a list of files.
    /// </summary>
    /// <param name="path">The directory path.</param>
    /// <param name="allowedFileExtensions">A list of file extensions to filter for.</param>
    /// <returns>An array of file names.</returns>
    string[] GetFiles(string path, string[] allowedFileExtensions);

    /// <summary>
    /// Returns the current directory the application is opened in.
    /// </summary>
    /// <returns>The current directory.</returns>
    string GetCurrentDirectory();
}
