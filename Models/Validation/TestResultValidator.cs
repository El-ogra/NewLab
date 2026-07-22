using FluentValidation;
using NewLab.Models.Domain;

namespace NewLab.Models.Validation
{
    public class TestResultValidator : AbstractValidator<TestResult>
    {
        public TestResultValidator()
        {
            RuleFor(r => r.PatientTestId).GreaterThan(0);
            RuleFor(r => r.LabTestElementId).GreaterThan(0);
            RuleFor(r => r.Value).MaximumLength(100);
            RuleFor(r => r.Comment).MaximumLength(500);
        }
    }
}
