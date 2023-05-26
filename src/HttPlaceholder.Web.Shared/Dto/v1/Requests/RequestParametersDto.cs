using System.Text;
using AutoMapper;
using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Web.Shared.Dto.v1.Requests;

/// <summary>
///     A model for storing the request data for a request.
/// </summary>
public class RequestParametersDto : IHaveCustomMapping
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
    ///     Gets or sets whether the request body is binary.
    /// </summary>
    public bool BodyIsBinary { get; set; }

    /// <summary>
    ///     Gets or sets the headers.
    /// </summary>
    public IDictionary<string, string> Headers { get; set; }

    /// <summary>
    ///     Gets or sets the client ip.
    /// </summary>
    public string ClientIp { get; set; }

    /// <inheritdoc />
    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<RequestParametersDto, RequestParametersModel>();
        configuration.CreateMap<RequestParametersModel, RequestParametersDto>()
            .ForMember(dest => dest.Body, opt => opt.Ignore())
            .AfterMap((src, dest, _) =>
            {
                var isBinary = !src.BinaryBody.IsValidAscii();
                dest.BodyIsBinary = isBinary;
                dest.Body = isBinary ? Convert.ToBase64String(src.BinaryBody) : Encoding.UTF8.GetString(src.BinaryBody);
            });
    }
}
