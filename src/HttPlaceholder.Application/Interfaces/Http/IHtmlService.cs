using HtmlAgilityPack;

namespace HttPlaceholder.Application.Interfaces.Http;

/// <summary>
///     Describes a class that is used to work with and manipulate HTML.
/// </summary>
public interface IHtmlService
{
    /// <summary>
    ///     Reads an HTML string and converts it to an <see cref="HtmlDocument"/>.
    /// </summary>
    /// <param name="html">The HTML string.</param>
    /// <returns>The converted <see cref="HtmlDocument"/>.</returns>
    HtmlDocument ReadHtml(string html);
}
