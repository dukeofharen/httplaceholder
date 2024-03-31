using HtmlAgilityPack;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;

namespace HttPlaceholder.Infrastructure.Web;

/// <summary>
///     A class that is used to work with and manipulate HTML.
/// </summary>
public class HtmlService : IHtmlService, ISingletonService
{
    /// <inheritdoc />
    public HtmlDocument ReadHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        return doc;
    }
}
