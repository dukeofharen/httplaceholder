﻿using static HttPlaceholder.Client.Verification.Dto.TimesModel;

namespace HttPlaceholder.Client.Tests.Verification.Dto;

[TestClass]
public class TimesModelFacts
{
    [TestMethod]
    public void Never_HappyFlow()
    {
        // Act
        var result = Never();

        // Assert
        Assert.AreEqual(0, result.ExactHits);
    }

    [TestMethod]
    public void ExactlyOnce_HappyFlow()
    {
        // Act
        var result = ExactlyOnce();

        // Assert
        Assert.AreEqual(1, result.ExactHits);
    }

    [TestMethod]
    public void AtLeastOnce_HappyFlow()
    {
        // Act
        var result = AtLeastOnce();

        // Assert
        Assert.AreEqual(1, result.MinHits);
    }

    [TestMethod]
    public void AtMostOnce_HappyFlow()
    {
        // Act
        var result = AtMostOnce();

        // Assert
        Assert.AreEqual(1, result.MaxHits);
    }

    [TestMethod]
    public void Exactly_HappyFlow()
    {
        // Arrange
        const int input = 2;

        // Act
        var result = Exactly(input);

        // Assert
        Assert.AreEqual(input, result.ExactHits);
    }

    [TestMethod]
    public void AtLeast_HappyFlow()
    {
        // Arrange
        const int input = 2;

        // Act
        var result = AtLeast(input);

        // Assert
        Assert.AreEqual(input, result.MinHits);
    }

    [TestMethod]
    public void AtMost_HappyFlow()
    {
        // Arrange
        const int input = 2;

        // Act
        var result = AtMost(input);

        // Assert
        Assert.AreEqual(input, result.MaxHits);
    }

    [TestMethod]
    public void Between_HappyFlow()
    {
        // Arrange
        const int atLeast = 2;
        const int atMost = 3;

        // Act
        var result = Between(atLeast, atMost);

        // Assert
        Assert.AreEqual(atLeast, result.MinHits);
        Assert.AreEqual(atMost, result.MaxHits);
    }
}
