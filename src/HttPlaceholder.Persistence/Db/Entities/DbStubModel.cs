﻿namespace HttPlaceholder.Persistence.Db.Entities;

/// <summary>
///     Represents a stub in the database.
/// </summary>
public class DbStubModel
{
    /// <summary>
    ///     Gets or sets the ID of the stub in the database.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     Gets or sets the distribution key.
    /// </summary>
    public string DistributionKey { get; set; }

    /// <summary>
    ///     Gets or sets the stub ID.
    /// </summary>
    public string StubId { get; set; }

    /// <summary>
    ///     Gets or sets the stub contents.
    /// </summary>
    public string Stub { get; set; }

    /// <summary>
    ///     Gets or sets the stub type.
    /// </summary>
    public string StubType { get; set; }
}
