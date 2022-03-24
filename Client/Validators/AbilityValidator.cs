using DMAdvantage.Shared.Entities;
using FluentValidation;
using MudBlazor;

namespace DMAdvantage.Client.Validators
{
    public class AbilityValidator : AbstractValidator<Ability>
    {
        public ISnackbar? Snackbar { get; set; }

        public AbilityValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<Ability>.CreateWithOptions((Ability)model, x => x.IncludeProperties(propertyName)));
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