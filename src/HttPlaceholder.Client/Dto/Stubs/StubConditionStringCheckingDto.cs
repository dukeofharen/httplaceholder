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
    [JsonProperty("equals", NullValueHandling = NullValueHandling.Ignore)]
    public string StringEquals { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive equals check.
    /// </summary>
    [JsonProperty("equalsci", NullValueHandling = NullValueHandling.Ignore)]
    public string StringEqualsCi { get; set; }

    /// <summary>
    /// Gets or sets the notequals check.
    /// </summary>
    [JsonProperty("notequals", NullValueHandling = NullValueHandling.Ignore)]
    public string StringNotEquals { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive notequals check.
    /// </summary>
    [JsonProperty("notequalsci", NullValueHandling = NullValueHandling.Ignore)]
    public string StringNotEqualsCi { get; set; }

    /// <summary>
    /// Gets or sets the contains check.
    /// </summary>
    [JsonProperty("contains", NullValueHandling = NullValueHandling.Ignore)]
    public string Contains { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive contains check.
    /// </summary>
    [JsonProperty("containsci", NullValueHandling = NullValueHandling.Ignore)]
    public string ContainsCi { get; set; }

    /// <summary>
    /// Gets or sets the not contains check.
    /// </summary>
    [JsonProperty("notcontains", NullValueHandling = NullValueHandling.Ignore)]
    public string NotContains { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive not contains check.
    /// </summary>
    [JsonProperty("notcontainsci", NullValueHandling = NullValueHandling.Ignore)]
    public string NotContainsCi { get; set; }

    /// <summary>
    /// Gets or sets the startswith check.
    /// </summary>
    [JsonProperty("startswith", NullValueHandling = NullValueHandling.Ignore)]
    public string StartsWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive startswith check.
    /// </summary>
    [JsonProperty("startswithci", NullValueHandling = NullValueHandling.Ignore)]
    public string StartsWithCi { get; set; }

    /// <summary>
    /// Gets or sets the doesnotstartwith check.
    /// </summary>
    [JsonProperty("doesnotstartwith", NullValueHandling = NullValueHandling.Ignore)]
    public string DoesNotStartWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive doesnotstartwith check.
    /// </summary>
    [JsonProperty("doesnotstartwithci", NullValueHandling = NullValueHandling.Ignore)]
    public string DoesNotStartWithCi { get; set; }

    /// <summary>
    /// Gets or sets the endswith check.
    /// </summary>
    [JsonProperty("endswith", NullValueHandling = NullValueHandling.Ignore)]
    public string EndsWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive endswith check.
    /// </summary>
    [JsonProperty("endswithci", NullValueHandling = NullValueHandling.Ignore)]
    public string EndsWithCi { get; set; }

    /// <summary>
    /// Gets or sets the doesnotendwith check.
    /// </summary>
    [JsonProperty("doesnotendwith", NullValueHandling = NullValueHandling.Ignore)]
    public string DoesNotEndWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive doesnotstartwith check.
    /// </summary>
    [JsonProperty("doesnotendwithci", NullValueHandling = NullValueHandling.Ignore)]
    public string DoesNotEndWithCi { get; set; }

    /// <summary>
    /// Gets or sets the regex check.
    /// </summary>
    [JsonProperty("regex", NullValueHandling = NullValueHandling.Ignore)]
    public string Regex { get; set; }

    /// <summary>
    /// Gets or sets the regexnomatches check.
    /// </summary>
    [JsonProperty("regexnomatches", NullValueHandling = NullValueHandling.Ignore)]
    public string RegexNoMatches { get; set; }

    /// <summary>
    /// Gets or sets the present check.
    /// </summary>
    [JsonProperty("present", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Present { get; set; }

    /// <summary>
    /// Gets or sets the min length check.
    /// </summary>
    [JsonProperty("minlength", NullValueHandling = NullValueHandling.Ignore)]
    public int? MinLength { get; set; }

    /// <summary>
    /// Gets or sets the max length check.
    /// </summary>
    [JsonProperty("maxlength", NullValueHandling = NullValueHandling.Ignore)]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets the exact length check.
    /// </summary>
    [JsonProperty("exactlength", NullValueHandling = NullValueHandling.Ignore)]
    public int? ExactLength { get; set; }
}
