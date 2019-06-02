using System.Reflection;
using AutoMapper;

namespace HttPlaceholder.Application.Infrastructure.AutoMapper
{
    public class WebAutoMapperProfile : Profile
    {
        public WebAutoMapperProfile()
        {
            this.InitializeProfile(Assembly.GetExecutingAssembly());
        }
    }
}
