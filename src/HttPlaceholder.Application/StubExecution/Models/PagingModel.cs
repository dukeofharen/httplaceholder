namespace HttPlaceholder.Application.StubExecution.Models;

/// <summary>
///     A model for providing information about paging.
/// </summary>
public class PagingModel
{
    /// <summary>
    ///     The identifier from which to find items. If this is not set; means to query from the start.
    /// </summary>
    public string FromIdentifier { get; private set; }

    /// <summary>
    ///     The number of items to show on a page.
    /// </summary>
    public int? ItemsPerPage { get; private set; }

    /// <summary>
    ///     Creates a <see cref="PagingModel"/> instance.
    /// </summary>
    /// <param name="fromIdentifier">The from identifier.</param>
    /// <param name="itemsPerPage">The number of items per page.</param>
    /// <returns>THe <see cref="PagingModel"/> or null if both parameters are not set.</returns>
    public static PagingModel Create(string fromIdentifier, int? itemsPerPage) =>
        !string.IsNullOrWhiteSpace(fromIdentifier) || itemsPerPage.HasValue
            ? new PagingModel { FromIdentifier = fromIdentifier, ItemsPerPage = itemsPerPage }
            : null;
}
