﻿using System.ComponentModel.DataAnnotations;

namespace HttPlaceholder.Common.Validation
{
    public class BetweenAttribute : ValidationAttribute
    {
        private readonly int _minValue;
        private readonly int _maxValue;
        private readonly bool _including;

        public BetweenAttribute(int minValue, int maxValue, bool including = false)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _including = including;
        }

        public override bool IsValid(object value)
        {
            var castedValue = (int?)value;
            return castedValue >= _minValue && (_including ? castedValue <= _maxValue : castedValue < _maxValue);
        }

        public override string FormatErrorMessage(string name) =>
            $"Field '{name}' should be between '{_minValue}' and '{(_including ? _maxValue : _maxValue - 1)}'.";
    }
}
