using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.StubBuilders;

namespace HttPlaceholder.Client.Utilities;

/// <summary>
/// A static class that contains several shortcut methods to quickly create DTOs for use in stubs.
/// </summary>
public static class DtoExtensions
{
    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "string equals" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto StringEquals(string input) =>
        StringCheckingDtoBuilder.Begin().StringEquals(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "string equals case insensitive" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto StringEqualsCaseInsensitive(string input) =>
        StringCheckingDtoBuilder.Begin().StringEqualsCaseInsensitive(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "string not equals" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto StringNotEquals(string input) =>
        StringCheckingDtoBuilder.Begin().StringNotEquals(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "string not equals case insensitive" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto StringNotEqualsCaseInsensitive(string input) =>
        StringCheckingDtoBuilder.Begin().StringNotEqualsCaseInsensitive(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "contains" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto Contains(string input) =>
        StringCheckingDtoBuilder.Begin().Contains(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "contains case insensitive" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto ContainsCaseInsensitive(string input) =>
        StringCheckingDtoBuilder.Begin().ContainsCaseInsensitive(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "not contains" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto NotContains(string input) =>
        StringCheckingDtoBuilder.Begin().NotContains(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "not contains case insensitive" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto NotContainsCaseInsensitive(string input) =>
        StringCheckingDtoBuilder.Begin().NotContainsCaseInsensitive(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "starts with" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto StartsWith(string input) =>
        StringCheckingDtoBuilder.Begin().StartsWith(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "starts with case insensitive" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto StartsWithCaseInsensitive(string input) =>
        StringCheckingDtoBuilder.Begin().StartsWithCaseInsensitive(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "does not start with" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto DoesNotStartWith(string input) =>
        StringCheckingDtoBuilder.Begin().DoesNotStartWith(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "does not start with case insensitive" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto DoesNotStartWithCaseInsensitive(string input) =>
        StringCheckingDtoBuilder.Begin().DoesNotStartWithCaseInsensitive(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "ends with" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto EndsWith(string input) =>
        StringCheckingDtoBuilder.Begin().EndsWith(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "ends with case insensitive" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto EndsWithCaseInsensitive(string input) =>
        StringCheckingDtoBuilder.Begin().EndsWithCaseInsensitive(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "does not end with" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto DoesNotEndWith(string input) =>
        StringCheckingDtoBuilder.Begin().DoesNotEndWith(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "does not end with case insensitive" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto DoesNotEndWithCaseInsensitive(string input) =>
        StringCheckingDtoBuilder.Begin().DoesNotEndWithCaseInsensitive(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "regex" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto Regex(string input) =>
        StringCheckingDtoBuilder.Begin().Regex(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "regex no matches" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto RegexNoMatches(string input) =>
        StringCheckingDtoBuilder.Begin().RegexNoMatches(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "min length" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto MinLength(int input) =>
        StringCheckingDtoBuilder.Begin().MinLength(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "max length" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto MaxLength(int input) =>
        StringCheckingDtoBuilder.Begin().MaxLength(input).Build();

    /// <summary>
    /// Returns a <see cref="StubConditionStringCheckingDto"/> with "exact length" check.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>The <see cref="StubConditionStringCheckingDto"/>.</returns>
    public static StubConditionStringCheckingDto ExactLength(int input) =>
        StringCheckingDtoBuilder.Begin().ExactLength(input).Build();
}
