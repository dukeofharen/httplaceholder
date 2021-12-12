using System.Reflection;
using AutoMapper;

namespace HttPlaceholder.Application.Infrastructure.AutoMapper;

public class ApplicationAutoMapperProfile : Profile
{
    public ApplicationAutoMapperProfile()
    {
        this.InitializeProfile(Assembly.GetExecutingAssembly());
    }
}