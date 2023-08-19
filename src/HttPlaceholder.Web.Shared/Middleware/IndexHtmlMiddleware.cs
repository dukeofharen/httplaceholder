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
    internal static string IndexHtml;
    private readonly IFileService _fileService;
    private readonly string _guiPath;
    private readonly IHtmlService _htmlService;
    private readonly IHttpContextService _httpContextService;
    private readonly RequestDelegate _next;
    private readonly IUrlResolver _urlResolver;

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
            if (string.IsNullOrWhiteSpace(IndexHtml))
            {
                var indexHtml =
                    await _fileService.ReadAllTextAsync(Path.Join(_guiPath, "index.html"), cancellationToken);
                var rootUrl = _urlResolver.GetRootUrl();
                var doc = _htmlService.ReadHtml(indexHtml);
                var headNode = doc.DocumentNode.SelectSingleNode("//html/head");
                headNode.PrependChild(HtmlNode.CreateNode(
                    @$"<script type=""text/javascript"">window.rootUrl = ""{rootUrl}"";</script>"));
                headNode.PrependChild(HtmlNode.CreateNode(@$"<base href=""{rootUrl}"">"));
                IndexHtml = doc.DocumentNode.OuterHtml;
            }

            _httpContextService.AddHeader(HeaderKeys.ContentType, MimeTypes.HtmlMime);
            await _httpContextService.WriteAsync(IndexHtml, cancellationToken);
        }
        else
        {
            await _next(context);
        }
    }
}
