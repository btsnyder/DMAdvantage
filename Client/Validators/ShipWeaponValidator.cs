using DMAdvantage.Shared.Entities;
using FluentValidation;

namespace DMAdvantage.Client.Validators
{
    public class ShipWeaponValidator : BaseValidator<ShipWeapon>
    {
        public ShipWeaponValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}