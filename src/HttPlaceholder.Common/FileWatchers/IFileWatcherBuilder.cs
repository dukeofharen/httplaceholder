﻿using System;
using System.Collections.Generic;
using System.IO;

namespace HttPlaceholder.Common.FileWatchers;

/// <summary>
///     Describes a class that is used to build a <see cref="FileSystemWatcher" />.
/// </summary>
public interface IFileWatcherBuilder
{
    /// <summary>
    ///     Sets the <see cref="NotifyFilters" />.
    /// </summary>
    /// <param name="filters">The <see cref="NotifyFilters" /></param>
    IFileWatcherBuilder SetNotifyFilters(NotifyFilters filters);

    /// <summary>
    ///     Sets the file watcher path or extension filters.
    ///     When the provided path is a file, the filter will be set to this file specifically.
    ///     If the path is a directory, the path will be set to the directory and the extensions provided
    ///     (provided in a list like this: ".md", ".yml" etc.) will be added as filters. If no extensions are provided,
    ///     the whole directory will be taken in account.
    /// </summary>
    /// <param name="path">The file watcher path.</param>
    /// <param name="extensions">The file extensions to watch if the path is a directory.</param>
    IFileWatcherBuilder SetPathOrFilters(string path, IEnumerable<string> extensions = null);

    /// <summary>
    ///     Enables or disables the "EnableRaisingEvents" property on the file watcher.
    /// </summary>
    /// <param name="enableRaisingEvents"></param>
    IFileWatcherBuilder SetEnableRaisingEvents(bool enableRaisingEvents);

    /// <summary>
    ///     Provides an action to executed when the Changed event is triggered.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    IFileWatcherBuilder SetOnChanged(Action<object, FileSystemEventArgs> action);

    /// <summary>
    ///     Provides an action to executed when the Created event is triggered.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    IFileWatcherBuilder SetOnCreated(Action<object, FileSystemEventArgs> action);

    /// <summary>
    ///     Provides an action to executed when the Deleted event is triggered.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    IFileWatcherBuilder SetOnDeleted(Action<object, FileSystemEventArgs> action);

    /// <summary>
    ///     Provides an action to executed when the Renamed event is triggered.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    IFileWatcherBuilder SetOnRenamed(Action<object, RenamedEventArgs> action);

    /// <summary>
    ///     Provides an action to executed when the Error event is triggered.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    IFileWatcherBuilder SetOnError(Action<object, ErrorEventArgs> action);

    /// <summary>
    ///     Builds the <see cref="FileSystemWatcher" />.
    /// </summary>
    /// <returns>The <see cref="FileSystemWatcher" />.</returns>
    FileSystemWatcher Build();
}
