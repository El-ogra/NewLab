using FluentValidation;
using NewLab.Models.Domain;
using NewLab.Models.Domain.Enums;

namespace NewLab.Models.Validation
{
    public class PatientValidator : AbstractValidator<Patient>
    {
        public PatientValidator()
        {
            RuleFor(p => p.FullName)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(p => p.AgeValue)
                .GreaterThanOrEqualTo(0);

            RuleFor(p => p.Gender)
                .IsInEnum()
                .Must(g => g == Gender.Male || g == Gender.Female);

            RuleFor(p => p.AgeValue)
                .InclusiveBetween(1m, 29m)
                .When(p => p.AgeUnit == AgeUnit.Day);

            RuleFor(p => p.AgeValue)
                .InclusiveBetween(1m, 11m)
                .When(p => p.AgeUnit == AgeUnit.Month);

            RuleFor(p => p.AgeValue)
                .InclusiveBetween(0m, 120m)
                .When(p => p.AgeUnit == AgeUnit.Year);

            RuleFor(p => p.FastingHours)
                .InclusiveBetween(0, 48)
                .When(p => p.IsFasting);

            RuleFor(p => p.PhoneNumber)
                .MaximumLength(20);
        }
    }
}
