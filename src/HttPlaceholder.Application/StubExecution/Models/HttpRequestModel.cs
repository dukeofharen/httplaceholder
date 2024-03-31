using System.Collections.Generic;
using System.Text;
using AutoMapper;
using HttPlaceholder.Application.Infrastructure.AutoMapper;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.Models;

/// <summary>
///     A model that contains a representation of an HTTP request.
/// </summary>
public class HttpRequestModel : IHaveCustomMapping
{
    /// <summary>
    ///     Gets or sets the method.
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    ///     Gets or sets the URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    ///     Gets or sets the body.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    ///     Gets or sets the headers.
    /// </summary>
    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    /// <summary>
    ///     Gets or sets the client ip.
    /// </summary>
    public string ClientIp { get; set; }

    /// <inheritdoc />
    public void CreateMappings(Profile configuration) =>
        configuration.CreateMap<RequestParametersModel, HttpRequestModel>()
            .ForMember(dest => dest.Body, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                if (!string.IsNullOrWhiteSpace(src.Body))
                {
                    dest.Body = src.Body;
                }
                else if (src.BinaryBody != null && src.BinaryBody.IsValidAscii())
                {
                    dest.Body = Encoding.UTF8.GetString(src.BinaryBody);
                }
            });
}
