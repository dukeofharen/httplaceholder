using HtmlAgilityPack;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Web.Shared.Middleware;

/// <summary>
///     A piece of middleware for handling the index.html file for the UI.
/// </summary>
public class IndexHtmlMiddleware(
    RequestDelegate next,
    string guiPath,
    IHttpContextService httpContextService,
    IFileService fileService,
    IUrlResolver urlResolver,
    IHtmlService htmlService,
    IAssemblyService assemblyService)
{
    /// <summary>
    ///     Handles the middleware.
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        var path = httpContextService.Path;
        var parts = new[] { "/ph-ui", "/ph-ui/", "/ph-ui/index.html" };
        if (parts.Any(p => path.Equals(p, StringComparison.OrdinalIgnoreCase)))
        {
            var cancellationToken = context?.RequestAborted ?? CancellationToken.None;
            var indexHtml =
                await fileService.ReadAllTextAsync(Path.Join(guiPath, "index.html"), cancellationToken);
            var rootUrl = urlResolver.GetRootUrl();
            var doc = htmlService.ReadHtml(indexHtml);
            var headNode = doc.DocumentNode.SelectSingleNode("//html/head");
            headNode.PrependChild(HtmlNode.CreateNode(
                $"""<script type="text/javascript">window.rootUrl = "{rootUrl}";</script>"""));
            headNode.PrependChild(HtmlNode.CreateNode($"""<base href="{rootUrl}/ph-ui/">"""));
            headNode.PrependChild(HtmlNode.CreateNode($"""<meta name="httplaceholder:version" content="{assemblyService.GetAssemblyVersion()}"/>"""));
            httpContextService.AddHeader(HeaderKeys.ContentType, MimeTypes.HtmlMime);
            await httpContextService.WriteAsync(doc.DocumentNode.OuterHtml, cancellationToken);
        }
        else
        {
            await next(context);
        }
    }
}
