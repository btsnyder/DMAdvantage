using DMAdvantage.Shared.Entities;
using FluentValidation;

namespace DMAdvantage.Client.Validators
{
    public class WeaponPropertyValidator : BaseValidator<WeaponProperty>
    {
        public WeaponPropertyValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}