using System.Reflection;
using AutoMapper;

namespace HttPlaceholder.Application.Infrastructure.AutoMapper;

/// <summary>
/// An AutoMapper profile that searches for mappings with reflection.
/// </summary>
public class ApplicationAutoMapperProfile : Profile
{
    /// <summary>
    /// Constructs an <see cref="ApplicationAutoMapperProfile"/> instance.
    /// </summary>
    public ApplicationAutoMapperProfile()
    {
        this.InitializeProfile(Assembly.GetExecutingAssembly());
    }
}
