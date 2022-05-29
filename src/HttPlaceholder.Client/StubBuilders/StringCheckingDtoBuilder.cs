using HttPlaceholder.Client.Dto.Stubs;

namespace HttPlaceholder.Client.StubBuilders;

/// <summary>
/// A class for building a <see cref="StubConditionStringCheckingDto"/> in a fluent way.
/// </summary>
public sealed class StringCheckingDtoBuilder
{
    private readonly StubConditionStringCheckingDto _dto = new();

    /// <summary>
    /// Creates a new <see cref="StringCheckingDtoBuilder"/> instance.
    /// </summary>
    /// <returns>A <see cref="StringCheckingDtoBuilder"/> instance.</returns>
    public static StringCheckingDtoBuilder Begin() => new();

    /// <summary>
    /// Sets the "case sensitive equals" check.
    /// </summary>
    /// <param name="equals">The case sensitive equals check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder StringEquals(string equals)
    {
        _dto.StringEquals = equals;
        return this;
    }

    /// <summary>
    /// Sets the "non case sensitive equals" check.
    /// </summary>
    /// <param name="equalsCi">The non case sensitive equals check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder StringEqualsCaseInsensitive(string equalsCi)
    {
        _dto.StringEqualsCi = equalsCi;
        return this;
    }

    /// <summary>
    /// Sets the "case sensitive not equals" check.
    /// </summary>
    /// <param name="notEquals">The case sensitive equals check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder StringNotEquals(string notEquals)
    {
        _dto.StringNotEquals = notEquals;
        return this;
    }

    /// <summary>
    /// Sets the "non case sensitive not equals" check.
    /// </summary>
    /// <param name="notEqualsCi">The case sensitive equals check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder StringNotEqualsCaseInsensitive(string notEqualsCi)
    {
        _dto.StringNotEqualsCi = notEqualsCi;
        return this;
    }

    /// <summary>
    /// Sets the "case sensitive contains" check.
    /// </summary>
    /// <param name="contains">The case sensitive contains check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder Contains(string contains)
    {
        _dto.Contains = contains;
        return this;
    }

    /// <summary>
    /// Sets the "non case sensitive contains" check.
    /// </summary>
    /// <param name="containsCi">The non case sensitive contains check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder ContainsCaseInsensitive(string containsCi)
    {
        _dto.ContainsCi = containsCi;
        return this;
    }

    /// <summary>
    /// Sets the "case sensitive not contains" check.
    /// </summary>
    /// <param name="notContains">The case sensitive not contains check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder NotContains(string notContains)
    {
        _dto.NotContains = notContains;
        return this;
    }

    /// <summary>
    /// Sets the "non case sensitive not contains" check.
    /// </summary>
    /// <param name="notContainsCi">The non case sensitive not contains check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder NotContainsCaseInsensitive(string notContainsCi)
    {
        _dto.NotContainsCi = notContainsCi;
        return this;
    }

    /// <summary>
    /// Sets the "case sensitive starts with" check.
    /// </summary>
    /// <param name="startsWith">The case sensitive starts with check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder StartsWith(string startsWith)
    {
        _dto.StartsWith = startsWith;
        return this;
    }

    /// <summary>
    /// Sets the "non case sensitive starts with" check.
    /// </summary>
    /// <param name="startsWithCi">The case sensitive starts with check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder StartsWithCaseInsensitive(string startsWithCi)
    {
        _dto.StartsWithCi = startsWithCi;
        return this;
    }

    /// <summary>
    /// Sets the "case sensitive not starts with" check.
    /// </summary>
    /// <param name="doesNotStartWith">The case sensitive not starts with check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder DoesNotStartWith(string doesNotStartWith)
    {
        _dto.DoesNotStartWith = doesNotStartWith;
        return this;
    }

    /// <summary>
    /// Sets the "non case sensitive not starts with" check.
    /// </summary>
    /// <param name="doesNotStartWithCi">The non case sensitive not starts with check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder DoesNotStartWithCaseInsensitive(string doesNotStartWithCi)
    {
        _dto.DoesNotStartWithCi = doesNotStartWithCi;
        return this;
    }

    /// <summary>
    /// Sets the "case sensitive ends with" check.
    /// </summary>
    /// <param name="endsWith">The case sensitive ends with check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder EndsWith(string endsWith)
    {
        _dto.EndsWith = endsWith;
        return this;
    }

    /// <summary>
    /// Sets the "non case sensitive ends with" check.
    /// </summary>
    /// <param name="endsWithCi">The case sensitive ends with check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder EndsWithCaseInsensitive(string endsWithCi)
    {
        _dto.EndsWithCi = endsWithCi;
        return this;
    }

    /// <summary>
    /// Sets the "case sensitive not ends with" check.
    /// </summary>
    /// <param name="doesNotEndWith">The case sensitive not ends with check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder DoesNotEndWith(string doesNotEndWith)
    {
        _dto.DoesNotEndWith = doesNotEndWith;
        return this;
    }

    /// <summary>
    /// Sets the "non case sensitive not ends with" check.
    /// </summary>
    /// <param name="doesNotEndWithCi">The non case sensitive not ends with check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder DoesNotEndWithCaseInsensitive(string doesNotEndWithCi)
    {
        _dto.DoesNotEndWithCi = doesNotEndWithCi;
        return this;
    }

    /// <summary>
    /// Sets the regex check.
    /// </summary>
    /// <param name="regex">The regex check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder Regex(string regex)
    {
        _dto.Regex = regex;
        return this;
    }

    /// <summary>
    /// Sets the regex no matches check.
    /// </summary>
    /// <param name="regexNoMatches">The regex no matches check.</param>
    /// <returns>The current <see cref="StringCheckingDtoBuilder"/>.</returns>
    public StringCheckingDtoBuilder RegexNoMatches(string regexNoMatches)
    {
        _dto.RegexNoMatches = regexNoMatches;
        return this;
    }

    /// <summary>
    /// Builds the string checking DTO.
    /// </summary>
    /// <returns>The built <see cref="StubConditionStringCheckingDto"/>.</returns>
    public StubConditionStringCheckingDto Build() => _dto;
}
