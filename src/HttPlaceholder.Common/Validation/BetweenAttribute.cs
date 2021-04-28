using System.ComponentModel.DataAnnotations;

namespace HttPlaceholder.Common.Validation
{
    public class BetweenAttribute : ValidationAttribute
    {
        private readonly int _minValue;
        private readonly int _maxValue;
        private readonly bool _including;
        private readonly bool _allowDefault;

        public BetweenAttribute(int minValue, int maxValue, bool including = false, bool allowDefault = false)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _including = including;
            _allowDefault = allowDefault;
        }

        public override bool IsValid(object value)
        {
            var castedValue = (int?)value;
            if (_allowDefault && (castedValue == 0 || castedValue == null))
            {
                return true;
            }

            return castedValue >= _minValue && (_including ? castedValue <= _maxValue : castedValue < _maxValue);
        }

        public override string FormatErrorMessage(string name) =>
            $"Field '{name}' should be between '{_minValue}' and '{(_including ? _maxValue : _maxValue - 1)}'.";
    }
}
