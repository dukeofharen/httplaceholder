﻿namespace HttPlaceholder.Application.Interfaces.Mappings;

/// <summary>
///     When a class implements this, a configuration will be added to AutoMapper for this class to be mapped from TEntity.
/// </summary>
/// <typeparam name="TEntity">The class this class can be mapped from.</typeparam>
public interface IMapFrom<TEntity>
{
}
