using DMAdvantage.Shared.Entities;
using FluentValidation;

namespace DMAdvantage.Client.Validators
{
    public class WeaponValidator : BaseValidator<Weapon>
    {
        public WeaponValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}