using System.ComponentModel.DataAnnotations;

namespace MyBestJob.BLL.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RequiredIfAttribute(string otherPropertyName, bool otherPropertyExpectedValue)
    : RequiredAttribute
{
    private readonly string _otherPropertyName = otherPropertyName;
    private readonly bool _otherPropertyExpectedValue = otherPropertyExpectedValue;

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var otherPropertyValue = (bool?)context.ObjectType
            .GetProperty(_otherPropertyName)?
            .GetValue(context.ObjectInstance);

        if (otherPropertyValue == _otherPropertyExpectedValue)
            if (!base.IsValid(value))
                return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }
}
