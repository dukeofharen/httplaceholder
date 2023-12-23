using System;
using System.Collections.Generic;
using System.IO;
using HttPlaceholder.Common;
using HttPlaceholder.Common.FileWatchers;

namespace HttPlaceholder.Infrastructure.Implementations.FileWatchers;

internal class FileWatcherBuilder(IFileService fileService) : IFileWatcherBuilder
{
    private readonly FileSystemWatcher _watcher = new() { EnableRaisingEvents = true };

    public void SetNotifyFilters(NotifyFilters filters) => _watcher.NotifyFilter = filters;

    public void SetPathOrFilters(string path, IEnumerable<string> extensions = null)
    {
        var isDir = fileService.IsDirectory(path);
        var finalLocation = (isDir ? path : Path.GetDirectoryName(path)) ??
                            throw new InvalidOperationException($"Location {path} is invalid.");
        _watcher.Path = finalLocation;
        if (!isDir)
        {
            _watcher.Filter = Path.GetFileName(path);
        }
        else if (extensions != null)
        {
            foreach (var extension in extensions)
            {
                _watcher.Filters.Add($"*{extension}");
            }
        }
    }

    public void SetOnChanged(Action<object, FileSystemEventArgs> action) =>
        _watcher.Changed += (sender, args) => action(sender, args);

    public void SetOnCreated(Action<object, FileSystemEventArgs> action) =>
        _watcher.Created += (sender, args) => action(sender, args);

    public void SetOnDeleted(Action<object, FileSystemEventArgs> action) =>
        _watcher.Deleted += (sender, args) => action(sender, args);

    public void SetOnRenamed(Action<object, RenamedEventArgs> action) =>
        _watcher.Renamed += (sender, args) => action(sender, args);

    public void SetOnError(Action<object, ErrorEventArgs> action) =>
        _watcher.Error += (sender, args) => action(sender, args);

    public FileSystemWatcher Build() => _watcher;
}
