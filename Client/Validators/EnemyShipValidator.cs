using DMAdvantage.Shared.Entities;
using FluentValidation;

namespace DMAdvantage.Client.Validators
{
    public class EnemyShipValidator : BaseValidator<EnemyShip>
    {
        private int[] _possibleHitDice = new int[] { 4, 6, 8, 10, 12 };

        public EnemyShipValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.HullHitDice).Must(x => _possibleHitDice.Contains(x));
            RuleFor(x => x.HullPoints).InclusiveBetween(0, 500);
            RuleFor(x => x.ShieldHitDice).Must(x => _possibleHitDice.Contains(x));
            RuleFor(x => x.ShieldPoints).InclusiveBetween(0, 500);
            RuleFor(x => x.ArmorClass).InclusiveBetween(0, 50);
            RuleFor(x => x.Speed).NotEmpty();
            RuleFor(x => x.Strength).InclusiveBetween(0, 20);
            RuleFor(x => x.StrengthBonus).InclusiveBetween(-10, 10);
            RuleFor(x => x.Dexterity).InclusiveBetween(0, 20);
            RuleFor(x => x.DexterityBonus).InclusiveBetween(-10, 10);
            RuleFor(x => x.Constitution).InclusiveBetween(0, 20);
            RuleFor(x => x.ConstitutionBonus).InclusiveBetween(-10, 10);
            RuleFor(x => x.Intelligence).InclusiveBetween(0, 20);
            RuleFor(x => x.IntelligenceBonus).InclusiveBetween(-10, 10);
            RuleFor(x => x.Wisdom).InclusiveBetween(0, 20);
            RuleFor(x => x.WisdomBonus).InclusiveBetween(-10, 10);
            RuleFor(x => x.Charisma).InclusiveBetween(0, 20);
            RuleFor(x => x.CharismaBonus).InclusiveBetween(-10, 10);
        }
    }
}
