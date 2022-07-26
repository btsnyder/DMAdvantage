using DMAdvantage.Shared.Entities;
using FluentValidation;

namespace DMAdvantage.Client.Validators
{
    public class ShipWeaponPropertyValidator : BaseValidator<ShipWeaponProperty>
    {
        public ShipWeaponPropertyValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}