using DMAdvantage.Shared.Entities;
using FluentValidation;
using MudBlazor;

namespace DMAdvantage.Client.Validators
{
    public class ClassValidator : AbstractValidator<DMClass>
    {
        public ISnackbar? Snackbar { get; set; }
        private readonly int[] _possibleHitDice = new int[] { 6, 8, 10, 12 };

        public ClassValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.HitDice).Must(x => _possibleHitDice.Contains(x));
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<DMClass>.CreateWithOptions((DMClass)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
            if (Snackbar != null)
            {
                foreach (var error in errors)
                {
                    Snackbar.Add(error, MudBlazor.Severity.Error);
                }
            }
            return errors;
        };
    }
}