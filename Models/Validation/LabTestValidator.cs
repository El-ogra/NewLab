using FluentValidation;
using NewLab.Models.Domain;

namespace NewLab.Models.Validation
{
    public class LabTestValidator : AbstractValidator<LabTest>
    {
        public LabTestValidator()
        {
            RuleFor(l => l.Code)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(l => l.TestName)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(l => l.ReportNameLarge)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(l => l.PatientPrice)
                .GreaterThanOrEqualTo(0);

            RuleFor(l => l.LabToLabPrice)
                .GreaterThanOrEqualTo(0);

            RuleFor(l => l.ExternalCost)
                .GreaterThanOrEqualTo(0)
                .When(l => l.IsSentExternal);

            RuleFor(l => l.TestTimeDays)
                .GreaterThanOrEqualTo(0);

            RuleFor(l => l.ArrangeNumber)
                .GreaterThanOrEqualTo(0);
        }
    }
}
