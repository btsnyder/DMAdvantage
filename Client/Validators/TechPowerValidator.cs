using DMAdvantage.Shared.Entities;
using FluentValidation;

namespace DMAdvantage.Client.Validators
{
    public class TechPowerValidator : BaseValidator<TechPower>
    {
        public TechPowerValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Level).InclusiveBetween(0, 10);
            RuleFor(x => x.Duration).NotEmpty();
        }
    }
}