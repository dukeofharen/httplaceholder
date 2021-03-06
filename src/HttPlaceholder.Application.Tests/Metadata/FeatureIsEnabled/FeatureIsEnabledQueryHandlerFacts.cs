﻿using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Metadata.Queries.FeatureIsEnabled;
using HttPlaceholder.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.Metadata.FeatureIsEnabled
{
    [TestClass]
    public class FeatureIsEnabledQueryHandlerFacts
    {
        private readonly SettingsModel _settings = new SettingsModel();
        private FeatureIsEnabledQueryHandler _handler;

        [TestInitialize]
        public void Initialize() =>
            _handler = new FeatureIsEnabledQueryHandler(Options.Create(_settings));

        [TestMethod]
        public async Task Handle_UnknownFeatureFlag_ShouldThrowNotImplementedException()
        {
            // Arrange
            var query = new FeatureIsEnabledQuery((FeatureFlagType)100);

            // Act / Assert
            await Assert.ThrowsExceptionAsync<NotImplementedException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [TestMethod]
        public async Task Handle_Authentication_AuthenticationSectionIsNull_ShouldReturnFalse()
        {
            // Arrange
            _settings.Authentication = null;
            var query = new FeatureIsEnabledQuery(FeatureFlagType.Authentication);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.AreEqual(query.FeatureFlag, result.FeatureFlag);
            Assert.IsFalse(result.Enabled);
        }

        [DataTestMethod]
        [DataRow(null, null, false)]
        [DataRow("", "", false)]
        [DataRow("username", "", false)]
        [DataRow("", "password", false)]
        [DataRow("username", "password", true)]
        public async Task Handle_Authentication(string username, string password, bool expectedResult)
        {
            // Arrange
            _settings.Authentication = new AuthenticationSettingsModel {ApiPassword = password, ApiUsername = username};
            var query = new FeatureIsEnabledQuery(FeatureFlagType.Authentication);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.AreEqual(query.FeatureFlag, result.FeatureFlag);
            Assert.AreEqual(expectedResult, result.Enabled);
        }
    }
}
