using AutoMapper;
using HttPlaceholder.Application.Infrastructure.AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.Infrastructure.AutoMapper
{
    [TestClass]
    public class ApplicationAutoMapperProfileFacts
    {
        [TestMethod]
        public void ApplicationAutoMapperProfile_ShouldBeConfiguredCorrectly()
        {
            // Arrange
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new ApplicationAutoMapperProfile()));

            // Act / Assert
            config.AssertConfigurationIsValid();
        }
    }
}
