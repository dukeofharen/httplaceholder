using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations;

internal class FileService : IFileService, ISingletonService
{
    /// <inheritdoc />
    public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken) => File.ReadAllBytesAsync(path, cancellationToken);

    /// <inheritdoc />
    public string ReadAllText(string path) => File.ReadAllText(path);

    /// <inheritdoc />
    public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken) => File.ReadAllTextAsync(path, cancellationToken);

    /// <inheritdoc />
    public Task WriteAllBytesAsync(string path, byte[] contents, CancellationToken cancellationToken) => File.WriteAllBytesAsync(path, contents, cancellationToken);

    /// <inheritdoc />
    public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);

    /// <inheritdoc />
    public Task WriteAllTextAsync(string path, string contents, CancellationToken cancellationToken) => File.WriteAllTextAsync(path, contents, cancellationToken);

    /// <inheritdoc />
    public bool FileExists(string path) => File.Exists(path);

    /// <inheritdoc />
    public Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken) => Task.Run(() => FileExists(path), cancellationToken);

    /// <inheritdoc />
    public bool DirectoryExists(string path) => Directory.Exists(path);

    /// <inheritdoc />
    public Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken) => Task.Run(() => DirectoryExists(path), cancellationToken);

    /// <inheritdoc />
    public Task CreateDirectoryAsync(string path, CancellationToken cancellationToken) => Task.Run(() => Directory.CreateDirectory(path), cancellationToken);

    /// <inheritdoc />
    public string GetTempPath() => Path.GetTempPath();

    /// <inheritdoc />
    public Task DeleteFileAsync(string path, CancellationToken cancellationToken) => Task.Run(() => File.Delete(path), cancellationToken);

    /// <inheritdoc />
    public DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);

    /// <inheritdoc />
    public Task<bool> IsDirectoryAsync(string path, CancellationToken cancellationToken) => Task.Run(() =>
        (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory, cancellationToken);

    /// <inheritdoc />
    public string[] GetFiles(string path, string searchPattern) => Directory.GetFiles(path, searchPattern);

    /// <inheritdoc />
    public Task<string[]> GetFilesAsync(string path, string searchPattern, CancellationToken cancellationToken) =>
        Task.Run(() => GetFiles(path, searchPattern), cancellationToken);

    /// <inheritdoc />
    public Task<string[]> GetFilesAsync(string path, IEnumerable<string> allowedFileExtensions, CancellationToken cancellationToken) =>
        Task.Run(() => Directory.GetFiles(path)
            .Where(f => allowedFileExtensions.Any(e => f.ToLower().EndsWith(e)))
            .ToArray(), cancellationToken);

    /// <inheritdoc />
    public string GetCurrentDirectory() => Directory.GetCurrentDirectory();
}
