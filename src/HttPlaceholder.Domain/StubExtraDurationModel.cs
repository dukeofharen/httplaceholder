﻿using YamlDotNet.Serialization;

namespace HttPlaceholder.Domain;

/// <summary>
///     A model for storing "extra duration" metadata.
/// </summary>
public class StubExtraDurationModel
{
    /// <summary>
    ///     Gets or sets the minimum duration.
    /// </summary>
    [YamlMember(Alias = "min")]
    public int? Min { get; set; }

    /// <summary>
    ///     Gets or sets the maximum duration.
    /// </summary>
    [YamlMember(Alias = "max")]
    public int? Max { get; set; }
}
