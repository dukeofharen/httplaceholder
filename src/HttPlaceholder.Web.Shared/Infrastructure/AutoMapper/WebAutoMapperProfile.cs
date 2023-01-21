using System.Reflection;
using AutoMapper;
using HttPlaceholder.Application.Infrastructure.AutoMapper;

namespace HttPlaceholder.Web.Shared.Infrastructure.AutoMapper;

/// <summary>
///     A class for registering all AutoMapper mappings in the web project.
/// </summary>
public class WebAutoMapperProfile : Profile
{
    /// <summary>
    ///     Constructs a <see cref="WebAutoMapperProfile" /> instance.
    /// </summary>
    public WebAutoMapperProfile()
    {
        this.InitializeProfile(Assembly.GetExecutingAssembly());
    }
}
