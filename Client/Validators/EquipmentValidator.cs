using DMAdvantage.Shared.Entities;
using FluentValidation;

namespace DMAdvantage.Client.Validators
{
    public class EquipmentValidator : BaseValidator<Equipment>
    {
        public EquipmentValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}