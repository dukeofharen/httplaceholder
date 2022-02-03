using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HttPlaceholder.Common.Validation;

/// <summary>
/// A validation attribute that checks if a property is equal to any of the given values.
/// </summary>
public class AnyOfFollowingAttribute : ValidationAttribute
{
    private readonly object[] _objectsToCheck;

    /// <summary>
    /// Constructs an <see cref="AnyOfFollowingAttribute"/> instance.
    /// </summary>
    /// <param name="objectsToCheck">The property to check should be equal to any of these values.</param>
    public AnyOfFollowingAttribute(params object[] objectsToCheck)
    {
        _objectsToCheck = objectsToCheck;
    }

    /// <inheritdoc />
    public override bool IsValid(object value) => value == null || _objectsToCheck.Contains(value);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        $"Value for '{name}' should be any of the following values: {string.Join(", ", _objectsToCheck)}.";
}
