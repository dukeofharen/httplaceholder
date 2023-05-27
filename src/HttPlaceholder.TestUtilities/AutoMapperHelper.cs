using AutoMapper;
using HttPlaceholder.Application.Infrastructure.AutoMapper;
using HttPlaceholder.Web.Shared.Infrastructure.AutoMapper;

namespace HttPlaceholder.TestUtilities;

public class AutoMapperHelper
{
    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<WebAutoMapperProfile>();
            cfg.AddProfile<ApplicationAutoMapperProfile>();
        });
        return config.CreateMapper();
    }
}
