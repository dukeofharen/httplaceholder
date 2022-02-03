using System.ComponentModel.DataAnnotations;

namespace HttPlaceholder.Common.Validation;

/// <summary>
/// A validation attribute that checks if a integer property is between a min and max value.
/// </summary>
public class BetweenAttribute : ValidationAttribute
{
    private readonly int _minValue;
    private readonly int _maxValue;
    private readonly bool _including;
    private readonly bool _allowDefault;

    /// <summary>
    /// Constructs an <see cref="BetweenAttribute"/> instance.
    /// </summary>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="maxValue">The maximum value/</param>
    /// <param name="including">True if the property to check can also be equal to the maximum value. False if the property should be 1 lower than the maximum value.</param>
    /// <param name="allowDefault">If true, allow the input parameter to be 0 or null.</param>
    public BetweenAttribute(int minValue, int maxValue, bool including = false, bool allowDefault = false)
    {
        _minValue = minValue;
        _maxValue = maxValue;
        _including = including;
        _allowDefault = allowDefault;
    }

    /// <inheritdoc />
    public override bool IsValid(object value)
    {
        var castedValue = (int?)value;
        if (_allowDefault && castedValue is 0 or null)
        {
            return true;
        }

        return castedValue >= _minValue && (_including ? castedValue <= _maxValue : castedValue < _maxValue);
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        $"Field '{name}' should be between '{_minValue}' and '{(_including ? _maxValue : _maxValue - 1)}'.";
}
