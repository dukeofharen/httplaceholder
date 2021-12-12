using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HttPlaceholder.Common.Validation;

public class AnyOfFollowingAttribute : ValidationAttribute
{
    private readonly object[] _objectsToCheck;

    public AnyOfFollowingAttribute(params object[] objectsToCheck)
    {
        _objectsToCheck = objectsToCheck;
    }

    public override bool IsValid(object value) => value == null || _objectsToCheck.Contains(value);

    public override string FormatErrorMessage(string name) =>
        $"Value for '{name}' should be any of the following values: {string.Join(", ", _objectsToCheck)}.";
}