using Newtonsoft.Json;

namespace HttPlaceholder.Client.Dto.Stubs;

/// <summary>
/// A model that contains several keywords for handling string checking.
/// </summary>
public class StubConditionStringCheckingDto
{
    /// <summary>
    /// Gets or sets the equals check.
    /// </summary>
    [JsonProperty("equals")]
    public string StringEquals { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive equals check.
    /// </summary>
    [JsonProperty("equalsci")]
    public string StringEqualsCi { get; set; }

    /// <summary>
    /// Gets or sets the notequals check.
    /// </summary>
    [JsonProperty("notequals")]
    public string StringNotEquals { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive notequals check.
    /// </summary>
    [JsonProperty("notequalsci")]
    public string StringNotEqualsCi { get; set; }

    /// <summary>
    /// Gets or sets the contains check.
    /// </summary>
    [JsonProperty("contains")]
    public string Contains { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive contains check.
    /// </summary>
    [JsonProperty("containsci")]
    public string ContainsCi { get; set; }

    /// <summary>
    /// Gets or sets the not contains check.
    /// </summary>
    [JsonProperty("notcontains")]
    public string NotContains { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive not contains check.
    /// </summary>
    [JsonProperty("notcontainsci")]
    public string NotContainsCi { get; set; }

    /// <summary>
    /// Gets or sets the startswith check.
    /// </summary>
    [JsonProperty("startswith")]
    public string StartsWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive startswith check.
    /// </summary>
    [JsonProperty("startswithci")]
    public string StartsWithCi { get; set; }

    /// <summary>
    /// Gets or sets the doesnotstartwith check.
    /// </summary>
    [JsonProperty("doesnotstartwith")]
    public string DoesNotStartWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive doesnotstartwith check.
    /// </summary>
    [JsonProperty("doesnotstartwithci")]
    public string DoesNotStartWithCi { get; set; }

    /// <summary>
    /// Gets or sets the endswith check.
    /// </summary>
    [JsonProperty("endswith")]
    public string EndsWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive endswith check.
    /// </summary>
    [JsonProperty("endswithci")]
    public string EndsWithCi { get; set; }

    /// <summary>
    /// Gets or sets the doesnotendwith check.
    /// </summary>
    [JsonProperty("doesnotendwith")]
    public string DoesNotEndWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive doesnotstartwith check.
    /// </summary>
    [JsonProperty("doesnotendwithci")]
    public string DoesNotEndWithCi { get; set; }

    /// <summary>
    /// Gets or sets the regex check.
    /// </summary>
    [JsonProperty("regex")]
    public string Regex { get; set; }

    /// <summary>
    /// Gets or sets the regexnomatches check.
    /// </summary>
    [JsonProperty("regexnomatches")]
    public string RegexNoMatches { get; set; }

    /// <summary>
    /// Gets or sets the present check.
    /// </summary>
    [JsonProperty("present")]
    public bool? Present { get; set; }

    /// <summary>
    /// Gets or sets the present check.
    /// </summary>
    [JsonProperty("notpresent")]
    public bool? NotPresent { get; set; }
}
