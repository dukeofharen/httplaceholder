using AutoMapper;
using HttPlaceholder.Application.Infrastructure.AutoMapper;
using HttPlaceholder.Web.Shared.Infrastructure.AutoMapper;

namespace HttPlaceholder.Web.Shared.Tests.Infrastructure;

[TestClass]
public class AutoMapperFacts
{
    [TestMethod]
    public void AutoMapper_AssertConfigurationIsValid()
    {
        // Arrange
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<WebAutoMapperProfile>();
            cfg.AddProfile<ApplicationAutoMapperProfile>();
        });

        // Act / Assert
        config.AssertConfigurationIsValid();
    }
}
