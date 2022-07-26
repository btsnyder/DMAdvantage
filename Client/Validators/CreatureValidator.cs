using DMAdvantage.Shared.Entities;
using FluentValidation;

namespace DMAdvantage.Client.Validators
{
    public class CreatureValidator : BaseValidator<Creature>
    {
        public CreatureValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.ChallengeRating).InclusiveBetween(0, 26);
            RuleFor(x => x.HitPoints).InclusiveBetween(0, 500);
            RuleFor(x => x.ArmorClass).InclusiveBetween(0, 50);
            RuleFor(x => x.Speed).NotEmpty();
            RuleFor(x => x.Strength).InclusiveBetween(0, 50);
            RuleFor(x => x.StrengthBonus).InclusiveBetween(-10, 10);
            RuleFor(x => x.Dexterity).InclusiveBetween(0, 50);
            RuleFor(x => x.DexterityBonus).InclusiveBetween(-10, 10);
            RuleFor(x => x.Constitution).InclusiveBetween(0, 50);
            RuleFor(x => x.ConstitutionBonus).InclusiveBetween(-10, 10);
            RuleFor(x => x.Intelligence).InclusiveBetween(0, 50);
            RuleFor(x => x.IntelligenceBonus).InclusiveBetween(-10, 10);
            RuleFor(x => x.Wisdom).InclusiveBetween(0, 50);
            RuleFor(x => x.WisdomBonus).InclusiveBetween(-10, 10);
            RuleFor(x => x.Charisma).InclusiveBetween(0, 50);
            RuleFor(x => x.CharismaBonus).InclusiveBetween(-10, 10);
            RuleFor(x => x.ForcePoints).InclusiveBetween(0, 50);
            RuleFor(x => x.TotalForcePowers).InclusiveBetween(0, 50);
            RuleFor(x => x.MaxForcePowerLevel).InclusiveBetween(0, 10);
        }
    }
}
