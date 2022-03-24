using DMAdvantage.Shared.Entities;
using FluentValidation;
using MudBlazor;

namespace DMAdvantage.Client.Validators
{
    public class CharacterValidator : AbstractValidator<Character>
    {
        private int[] _possibleHitDice = new int[] { 6, 8, 10, 12 };
        public ISnackbar? Snackbar { get; set; }

        public CharacterValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.HitDice).Must(x => _possibleHitDice.Contains(x));
            RuleFor(x => x.HitPoints).InclusiveBetween(0, 500);
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
            RuleFor(x => x.ForcePoints).InclusiveBetween(0, 50);
            RuleFor(x => x.TotalForcePowers).InclusiveBetween(0, 50);
            RuleFor(x => x.MaxForcePowerLevel).InclusiveBetween(0, 10);
            RuleFor(x => x.Level).InclusiveBetween(1, 20);
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<Character>.CreateWithOptions((Character)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
            if (Snackbar == null) return errors;
            foreach (var error in errors)
            {
                Snackbar.Add(error, MudBlazor.Severity.Error);
            }
            return errors;
        };
    }
}
