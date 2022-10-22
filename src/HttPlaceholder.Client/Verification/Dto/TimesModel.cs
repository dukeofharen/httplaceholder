namespace HttPlaceholder.Client.Verification.Dto;

/// <summary>
///     A model used to store verification details about how much times a stub should have been called.
/// </summary>
public class TimesModel
{
    /// <summary>
    ///     The minimum (inclusive) amount of hits the stub should have been hit.
    /// </summary>
    public int? MinHits { get; set; }

    /// <summary>
    ///     The maximum (inclusive) amount of hits the stub should have been hit.
    /// </summary>
    public int? MaxHits { get; set; }

    /// <summary>
    ///     The exact amount of hits the stub should have been hit.
    /// </summary>
    public int? ExactHits { get; set; }

    /// <summary>
    ///     The stub should have never been hit.
    /// </summary>
    public static TimesModel Never() => new() {ExactHits = 0};

    /// <summary>
    ///     The stub should have been hit exactly once.
    /// </summary>
    public static TimesModel ExactlyOnce() => new() {ExactHits = 1};

    /// <summary>
    ///     The stub should have been hit at least once.
    /// </summary>
    public static TimesModel AtLeastOnce() => new() {MinHits = 1};

    /// <summary>
    ///     The stub should have been hit at most once.
    /// </summary>
    public static TimesModel AtMostOnce() => new() {MaxHits = 1};

    /// <summary>
    ///     The stub should have exactly been hit this amount of times.
    /// </summary>
    public static TimesModel Exactly(int exactly) => new() {ExactHits = exactly};

    /// <summary>
    ///     The stub should have at least been hit this amount of times.
    /// </summary>
    public static TimesModel AtLeast(int atLeast) => new() {MinHits = atLeast};

    /// <summary>
    ///     The stub should have at most been hit this amount of times.
    /// </summary>
    public static TimesModel AtMost(int atMost) => new() {MaxHits = atMost};

    /// <summary>
    ///     The stub should have been hit between these 2 numbers.
    /// </summary>
    public static TimesModel Between(int atLeast, int atMost) => new() {MinHits = atLeast, MaxHits = atMost};
}
