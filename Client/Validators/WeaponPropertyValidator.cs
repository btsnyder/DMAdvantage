using DMAdvantage.Shared.Entities;
using FluentValidation;
using MudBlazor;

namespace DMAdvantage.Client.Validators
{
    public class WeaponPropertyValidator : AbstractValidator<WeaponProperty>
    {
        public ISnackbar? Snackbar { get; set; }

        public WeaponPropertyValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<WeaponProperty>.CreateWithOptions((WeaponProperty)model, x => x.IncludeProperties(propertyName)));
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