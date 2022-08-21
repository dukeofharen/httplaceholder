using Newtonsoft.Json;
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
    [JsonProperty("equals", NullValueHandling = NullValueHandling.Ignore)]
    public string StringEquals { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive equals check.
    /// </summary>
    [YamlMember(Alias = "equalsci")]
    [JsonProperty("equalsci", NullValueHandling = NullValueHandling.Ignore)]
    public string StringEqualsCi { get; set; }

    /// <summary>
    /// Gets or sets the notequals check.
    /// </summary>
    [YamlMember(Alias = "notequals")]
    [JsonProperty("notequals", NullValueHandling = NullValueHandling.Ignore)]
    public string StringNotEquals { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive notequals check.
    /// </summary>
    [YamlMember(Alias = "notequalsci")]
    [JsonProperty("notequalsci", NullValueHandling = NullValueHandling.Ignore)]
    public string StringNotEqualsCi { get; set; }

    /// <summary>
    /// Gets or sets the contains check.
    /// </summary>
    [YamlMember(Alias = "contains")]
    [JsonProperty("contains", NullValueHandling = NullValueHandling.Ignore)]
    public string Contains { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive contains check.
    /// </summary>
    [YamlMember(Alias = "containsci")]
    [JsonProperty("containsci", NullValueHandling = NullValueHandling.Ignore)]
    public string ContainsCi { get; set; }

    /// <summary>
    /// Gets or sets the not contains check.
    /// </summary>
    [YamlMember(Alias = "notcontains")]
    [JsonProperty("notcontains", NullValueHandling = NullValueHandling.Ignore)]
    public string NotContains { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive not contains check.
    /// </summary>
    [YamlMember(Alias = "notcontainsci")]
    [JsonProperty("notcontainsci", NullValueHandling = NullValueHandling.Ignore)]
    public string NotContainsCi { get; set; }

    /// <summary>
    /// Gets or sets the startswith check.
    /// </summary>
    [YamlMember(Alias = "startswith")]
    [JsonProperty("startswith", NullValueHandling = NullValueHandling.Ignore)]
    public string StartsWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive startswith check.
    /// </summary>
    [YamlMember(Alias = "startswithci")]
    [JsonProperty("startswithci", NullValueHandling = NullValueHandling.Ignore)]
    public string StartsWithCi { get; set; }

    /// <summary>
    /// Gets or sets the doesnotstartwith check.
    /// </summary>
    [YamlMember(Alias = "doesnotstartwith")]
    [JsonProperty("doesnotstartwith", NullValueHandling = NullValueHandling.Ignore)]
    public string DoesNotStartWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive doesnotstartwith check.
    /// </summary>
    [YamlMember(Alias = "doesnotstartwithci")]
    [JsonProperty("doesnotstartwithci", NullValueHandling = NullValueHandling.Ignore)]
    public string DoesNotStartWithCi { get; set; }

    /// <summary>
    /// Gets or sets the endswith check.
    /// </summary>
    [YamlMember(Alias = "endswith")]
    [JsonProperty("endswith", NullValueHandling = NullValueHandling.Ignore)]
    public string EndsWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive endswith check.
    /// </summary>
    [YamlMember(Alias = "endswithci")]
    [JsonProperty("endswithci", NullValueHandling = NullValueHandling.Ignore)]
    public string EndsWithCi { get; set; }

    /// <summary>
    /// Gets or sets the doesnotendwith check.
    /// </summary>
    [YamlMember(Alias = "doesnotendwith")]
    [JsonProperty("doesnotendwith", NullValueHandling = NullValueHandling.Ignore)]
    public string DoesNotEndWith { get; set; }

    /// <summary>
    /// Gets or sets the non case sensitive doesnotstartwith check.
    /// </summary>
    [YamlMember(Alias = "doesnotendwithci")]
    [JsonProperty("doesnotendwithci", NullValueHandling = NullValueHandling.Ignore)]
    public string DoesNotEndWithCi { get; set; }

    /// <summary>
    /// Gets or sets the regex check.
    /// </summary>
    [YamlMember(Alias = "regex")]
    [JsonProperty("regex", NullValueHandling = NullValueHandling.Ignore)]
    public string Regex { get; set; }

    /// <summary>
    /// Gets or sets the regexnomatches check.
    /// </summary>
    [YamlMember(Alias = "regexnomatches")]
    [JsonProperty("regexnomatches", NullValueHandling = NullValueHandling.Ignore)]
    public string RegexNoMatches { get; set; }

    /// <summary>
    /// Gets or sets the present check.
    /// </summary>
    [YamlMember(Alias = "present")]
    [JsonProperty("present", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Present { get; set; }

    /// <summary>
    /// Gets or sets the min length check.
    /// </summary>
    [YamlMember(Alias = "minlength")]
    [JsonProperty("minlength", NullValueHandling = NullValueHandling.Ignore)]
    public int? MinLength { get; set; }

    /// <summary>
    /// Gets or sets the max length check.
    /// </summary>
    [YamlMember(Alias = "maxlength")]
    [JsonProperty("maxlength", NullValueHandling = NullValueHandling.Ignore)]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets the exact length check.
    /// </summary>
    [YamlMember(Alias = "exactlength")]
    [JsonProperty("exactlength", NullValueHandling = NullValueHandling.Ignore)]
    public int? ExactLength { get; set; }

    /// <inheritdoc />
    public override string ToString() => JsonConvert.SerializeObject(this);
}
