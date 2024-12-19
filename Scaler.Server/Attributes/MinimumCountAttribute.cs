using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Scaler.Server.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="minCount"></param>
    /// <param name="required"></param>
    /// <param name="allowEmptyStringValues"></param>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MinimumCountAttribute(int minCount, bool required = true, bool allowEmptyStringValues = false) :
        ValidationAttribute("'{0}' must have at least {1} item.")
    {
        public MinimumCountAttribute() : this(1)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, minCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            if (value == null)
                return !required;

            if (!allowEmptyStringValues && value is ICollection<string> stringList)
                return stringList.Count(s => !string.IsNullOrWhiteSpace(s)) >= minCount;

            if (value is ICollection list)
                return list.Count >= minCount;

            return false;
        }
    }
}
