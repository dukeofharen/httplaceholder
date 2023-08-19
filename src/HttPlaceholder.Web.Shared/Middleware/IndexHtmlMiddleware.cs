using HtmlAgilityPack;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Web.Shared.Middleware;

/// <summary>
///     A piece of middleware for handling the index.html file for the UI.
/// </summary>
public class IndexHtmlMiddleware
{
    private static string _indexHtml;
    private readonly RequestDelegate _next;
    private readonly string _guiPath;
    private readonly IHttpContextService _httpContextService;
    private readonly IFileService _fileService;
    private readonly IUrlResolver _urlResolver;
    private readonly IHtmlService _htmlService;

    /// <summary>
    ///     Constructs an <see cref="IndexHtmlMiddleware" /> instance.
    /// </summary>
    public IndexHtmlMiddleware(
        RequestDelegate next,
        string path,
        IHttpContextService httpContextService,
        IFileService fileService,
        IUrlResolver urlResolver,
        IHtmlService htmlService)
    {
        _next = next;
        _guiPath = path;
        _httpContextService = httpContextService;
        _fileService = fileService;
        _urlResolver = urlResolver;
        _htmlService = htmlService;
    }

    /// <summary>
    ///     Handles the middleware.
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        var path = _httpContextService.Path;
        var parts = new[] {"/ph-ui", "/ph-ui/", "/ph-ui/index.html"};
        if (parts.Any(p => path.Equals(p, StringComparison.OrdinalIgnoreCase)))
        {
            var cancellationToken = context?.RequestAborted ?? CancellationToken.None;
            if (string.IsNullOrWhiteSpace(_indexHtml))
            {
                var indexHtml = await _fileService.ReadAllTextAsync(Path.Join(_guiPath, "index.html"), cancellationToken);
                var rootUrl = _urlResolver.GetRootUrl();
                var doc = _htmlService.ReadHtml(indexHtml);
                var headNode = doc.DocumentNode.SelectSingleNode("//html/head");
                headNode.PrependChild(HtmlNode.CreateNode(
                    @$"<script type=""text/javascript"">window.rootUrl = ""{rootUrl}"";</script>"));
                headNode.PrependChild(HtmlNode.CreateNode(@$"<base href=""{rootUrl}"">"));
                _indexHtml = doc.DocumentNode.InnerHtml;
            }

            _httpContextService.AddHeader("Content-Type", MimeTypes.HtmlMime);
            await _httpContextService.WriteAsync(_indexHtml, cancellationToken);
        }
        else
        {
            await _next(context);
        }
    }
}
