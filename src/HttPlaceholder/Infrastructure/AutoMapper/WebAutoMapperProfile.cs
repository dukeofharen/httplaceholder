using System.Reflection;
using AutoMapper;
using HttPlaceholder.Application.Infrastructure.AutoMapper;

namespace HttPlaceholder.Infrastructure.AutoMapper;

public class WebAutoMapperProfile : Profile
{
    public WebAutoMapperProfile()
    {
        this.InitializeProfile(Assembly.GetExecutingAssembly());
    }
}