using FluentValidation;
using NewLab.Models.Domain;
using NewLab.Models.Domain.Enums;

namespace NewLab.Models.Validation
{
    public class NormalRangeValidator : AbstractValidator<NormalRange>
    {
        public NormalRangeValidator()
        {
            RuleFor(r => r.LabTestId).GreaterThan(0);
            RuleFor(r => r.TestName).NotEmpty().MaximumLength(200);
            RuleFor(r => r.Gender)
                .IsInEnum()
                .Must(g => g == Gender.Male || g == Gender.Female)
                .WithMessage("الجنس يجب أن يكون Male أو Female فقط");
            RuleFor(r => r.LowLimit).LessThanOrEqualTo(r => r.HighLimit)
                .WithMessage("Low limit يجب أن يكون أقل من أو يساوي High limit");
            RuleFor(r => r.AgeFrom).LessThanOrEqualTo(r => r.AgeTo)
                .WithMessage("Age From يجب أن يكون أقل من أو يساوي Age To");
            RuleFor(r => r.CriticalLowLimit)
                .LessThanOrEqualTo(r => r.LowLimit)
                .When(r => r.CriticalLowLimit.HasValue)
                .WithMessage("Critical Low يجب أن يكون أقل من أو يساوي Low limit");
            RuleFor(r => r.CriticalHighLimit)
                .GreaterThanOrEqualTo(r => r.HighLimit)
                .When(r => r.CriticalHighLimit.HasValue)
                .WithMessage("Critical High يجب أن يكون أكبر من أو يساوي High limit");
        }
    }
}
