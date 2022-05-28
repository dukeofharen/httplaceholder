using YamlDotNet.Serialization;

namespace HttPlaceholder.Domain;

/// <summary>
/// A model that contains several keywords for handling string checking.
/// </summary>
public class StubConditionStringCheckingModel
{
    /// <summary>
    /// Gets or sets the equals check.
    /// </summary>
    [YamlMember(Alias = "equals")]
    public string StringEquals { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive equals check.
    /// </summary>
    [YamlMember(Alias = "equalsci")]
    public string StringEqualsCi { get; set; }

    /// <summary>
    /// Gets or sets the notequals check.
    /// </summary>
    [YamlMember(Alias = "notequals")]
    public string StringNotEquals { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive notequals check.
    /// </summary>
    [YamlMember(Alias = "notequalsci")]
    public string StringNotEqualsCi { get; set; }

    /// <summary>
    /// Gets or sets the contains check.
    /// </summary>
    [YamlMember(Alias = "contains")]
    public string Contains { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive contains check.
    /// </summary>
    [YamlMember(Alias = "containsci")]
    public string ContainsCi { get; set; }

    /// <summary>
    /// Gets or sets the not contains check.
    /// </summary>
    [YamlMember(Alias = "notcontains")]
    public string NotContains { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive not contains check.
    /// </summary>
    [YamlMember(Alias = "notcontainsci")]
    public string NotContainsCi { get; set; }

    /// <summary>
    /// Gets or sets the startswith check.
    /// </summary>
    [YamlMember(Alias = "startswith")]
    public string StartsWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive startswith check.
    /// </summary>
    [YamlMember(Alias = "startswithci")]
    public string StartsWithCi { get; set; }

    /// <summary>
    /// Gets or sets the doesnotstartwith check.
    /// </summary>
    [YamlMember(Alias = "doesnotstartwith")]
    public string DoesNotStartWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive doesnotstartwith check.
    /// </summary>
    [YamlMember(Alias = "doesnotstartwithci")]
    public string DoesNotStartWithCi { get; set; }

    /// <summary>
    /// Gets or sets the endswith check.
    /// </summary>
    [YamlMember(Alias = "endswith")]
    public string EndsWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive endswith check.
    /// </summary>
    [YamlMember(Alias = "endswithci")]
    public string EndsWithCi { get; set; }

    /// <summary>
    /// Gets or sets the doesnotendwith check.
    /// </summary>
    [YamlMember(Alias = "doesnotendwith")]
    public string DoesNotEndWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive doesnotstartwith check.
    /// </summary>
    [YamlMember(Alias = "doesnotendwithci")]
    public string DoesNotEndWithCi { get; set; }

    /// <summary>
    /// Gets or sets the regex check.
    /// </summary>
    [YamlMember(Alias = "regex")]
    public string Regex { get; set; }

    /// <summary>
    /// Gets or sets the regexnomatches check.
    /// </summary>
    [YamlMember(Alias = "regexnomatches")]
    public string RegexNoMatches { get; set; }

    /// <summary>
    /// Gets or sets the present check.
    /// </summary>
    [YamlMember(Alias = "present")]
    public bool Present { get; set; }

    /// <summary>
    /// Gets or sets the present check.
    /// </summary>
    [YamlMember(Alias = "notpresent")]
    public bool NotPresent { get; set; }
}
