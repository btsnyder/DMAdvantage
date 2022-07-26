using DMAdvantage.Shared.Entities;
using FluentValidation;

namespace DMAdvantage.Client.Validators
{
    public class ClassValidator : BaseValidator<DMClass>
    {
        private readonly int[] _possibleHitDice = new int[] { 6, 8, 10, 12 };

        public ClassValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.HitDice).Must(x => _possibleHitDice.Contains(x));
        }
    }
}