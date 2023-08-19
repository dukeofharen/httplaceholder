using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.Models;

/// <summary>
///     A model that contains a representation of an HTTP response.
///     This model is mainly used in converting one data source (e.g. a HTTP Archive, or HAR) to an intermediate data
///     source
///     that can be used to generate a response for use in a stub.
/// </summary>
public class HttpResponseModel : IHaveCustomMapping
{
    /// <summary>
    ///     Gets or sets the HTTP status code.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    ///     Gets or sets the headers.
    /// </summary>
    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    /// <summary>
    ///     Gets or sets the HTTP response content.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    ///     Gets or sets whether the content is Base64 encoded.
    /// </summary>
    public bool ContentIsBase64 { get; set; }

    /// <inheritdoc />
    public void CreateMappings(Profile configuration) =>
        configuration.CreateMap<ResponseModel, HttpResponseModel>()
            .ForMember(src => src.Content, opt => opt.Ignore())
            .ForMember(src => src.ContentIsBase64, opt => opt.Ignore())
            .AfterMap((src, dest, _) =>
            {
                if (src.BodyIsBinary)
                {
                    dest.ContentIsBase64 = true;
                    dest.Content = Convert.ToBase64String(src.Body);
                }
                else
                {
                    dest.ContentIsBase64 = false;
                    dest.Content = Encoding.UTF8.GetString(src.Body);
                }
            });
}
