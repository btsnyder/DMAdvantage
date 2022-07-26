using DMAdvantage.Shared.Entities;
using FluentValidation;

namespace DMAdvantage.Client.Validators
{
    public class AbilityValidator : BaseValidator<Ability>
    {
        public AbilityValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}