namespace HttPlaceholder.Common;

/// <summary>
///     Describes a class that is used as cache.
/// </summary>
public interface ICacheService
{
    /// <summary>
    ///     Retrieves a cache item from the current scope (e.g. HttpContext).
    /// </summary>
    /// <param name="key">The item key.</param>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <returns>The object.</returns>
    TObject GetScopedItem<TObject>(string key);

    /// <summary>
    ///     Puts a cache item on the current scope (e.g. HttpContext).
    /// </summary>
    /// <param name="key">The item key.</param>
    /// <param name="item">The item to store.</param>
    void SetScopedItem(string key, object item);

    /// <summary>
    ///     Deletes an item on the current scope (e.g. HttpContext).
    /// </summary>
    /// <param name="key">The item key.</param>
    /// <returns>True if the item was found and deleted; false otherwise.</returns>
    bool DeleteScopedItem(string key);
}
