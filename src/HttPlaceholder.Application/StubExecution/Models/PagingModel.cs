namespace HttPlaceholder.Application.StubExecution.Models;

/// <summary>
///     A model for providing information about paging.
/// </summary>
public class PagingModel
{
    /// <summary>
    ///     The identifier from which to find items. If this is not set; means to query from the start.
    /// </summary>
    public string FromIdentifier { get; set; }

    /// <summary>
    ///     The number of items to show on a page.
    /// </summary>
    public int? ItemsPerPage { get; set; }
}
