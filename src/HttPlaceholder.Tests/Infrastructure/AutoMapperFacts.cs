using AutoMapper;
using HttPlaceholder.Application.Infrastructure.AutoMapper;
using HttPlaceholder.Infrastructure.AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Infrastructure
{
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
}
