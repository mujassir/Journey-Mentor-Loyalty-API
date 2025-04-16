using FluentValidation;
using JourneyMentor.Loyalty.Application.Features.Points.Commands;

namespace JourneyMentor.Loyalty.Application.Validators
{
    public class EarnPointsCommandValidator : AbstractValidator<EarnPointsCommand>
    {
        public EarnPointsCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Points).GreaterThan(0);
        }
    }
}
