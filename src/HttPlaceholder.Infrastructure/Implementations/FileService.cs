using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations;

/// <inheritdoc />
public class FileService : IFileService
{
    /// <inheritdoc />
    public Task<byte[]> ReadAllBytesAsync(string path) => File.ReadAllBytesAsync(path);

    /// <inheritdoc />
    public string ReadAllText(string path) => File.ReadAllText(path);

    /// <inheritdoc />
    public Task<string> ReadAllTextAsync(string path) => File.ReadAllTextAsync(path);

    /// <inheritdoc />
    public Task WriteAllBytesAsync(string path, byte[] contents) => File.WriteAllBytesAsync(path, contents);

    /// <inheritdoc />
    public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);

    /// <inheritdoc />
    public Task WriteAllTextAsync(string path, string contents) => File.WriteAllTextAsync(path, contents);

    /// <inheritdoc />
    public bool FileExists(string path) => File.Exists(path);

    /// <inheritdoc />
    public Task<bool> FileExistsAsync(string path) => Task.Run(() => File.Exists(path));

    /// <inheritdoc />
    public bool DirectoryExists(string path) => Directory.Exists(path);

    /// <inheritdoc />
    public Task<bool> DirectoryExistsAsync(string path) => Task.Run(() => Directory.Exists(path));

    /// <inheritdoc />
    public Task CreateDirectoryAsync(string path) => Task.Run(() => Directory.CreateDirectory(path));

    /// <inheritdoc />
    public string GetTempPath() => Path.GetTempPath();

    /// <inheritdoc />
    public Task DeleteFileAsync(string path) => Task.Run(() => File.Delete(path));

    /// <inheritdoc />
    public DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);

    /// <inheritdoc />
    public bool IsDirectory(string path) =>
        (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;

    /// <inheritdoc />
    public string[] GetFiles(string path, string searchPattern) => Directory.GetFiles(path, searchPattern);

    /// <inheritdoc />
    public string[] GetFiles(string path, string[] allowedFileExtensions) =>
        Directory.GetFiles(path)
            .Where(f => allowedFileExtensions.Any(e => f.ToLower().EndsWith(e)))
            .ToArray();

    /// <inheritdoc />
    public string GetCurrentDirectory() => Directory.GetCurrentDirectory();
}
