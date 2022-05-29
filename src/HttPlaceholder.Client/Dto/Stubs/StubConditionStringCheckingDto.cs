namespace HttPlaceholder.Client.Dto.Stubs;

/// <summary>
/// A model that contains several keywords for handling string checking.
/// </summary>
public class StubConditionStringCheckingDto
{
     /// <summary>
    /// Gets or sets the equals check.
    /// </summary>
    public string StringEquals { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive equals check.
    /// </summary>
    public string StringEqualsCi { get; set; }

    /// <summary>
    /// Gets or sets the notequals check.
    /// </summary>
    public string StringNotEquals { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive notequals check.
    /// </summary>
    public string StringNotEqualsCi { get; set; }

    /// <summary>
    /// Gets or sets the contains check.
    /// </summary>
    public string Contains { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive contains check.
    /// </summary>
    public string ContainsCi { get; set; }

    /// <summary>
    /// Gets or sets the not contains check.
    /// </summary>
    public string NotContains { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive not contains check.
    /// </summary>
    public string NotContainsCi { get; set; }

    /// <summary>
    /// Gets or sets the startswith check.
    /// </summary>
    public string StartsWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive startswith check.
    /// </summary>
    public string StartsWithCi { get; set; }

    /// <summary>
    /// Gets or sets the doesnotstartwith check.
    /// </summary>
    public string DoesNotStartWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive doesnotstartwith check.
    /// </summary>
    public string DoesNotStartWithCi { get; set; }

    /// <summary>
    /// Gets or sets the endswith check.
    /// </summary>
    public string EndsWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive endswith check.
    /// </summary>
    public string EndsWithCi { get; set; }

    /// <summary>
    /// Gets or sets the doesnotendwith check.
    /// </summary>
    public string DoesNotEndWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive doesnotstartwith check.
    /// </summary>
    public string DoesNotEndWithCi { get; set; }

    /// <summary>
    /// Gets or sets the regex check.
    /// </summary>
    public string Regex { get; set; }

    /// <summary>
    /// Gets or sets the regexnomatches check.
    /// </summary>
    public string RegexNoMatches { get; set; }

    /// <summary>
    /// Gets or sets the present check.
    /// </summary>
    public bool? Present { get; set; }

    /// <summary>
    /// Gets or sets the present check.
    /// </summary>
    public bool? NotPresent { get; set; }
}
