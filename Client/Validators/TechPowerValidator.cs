using DMAdvantage.Shared.Entities;
using FluentValidation;
using MudBlazor;

namespace DMAdvantage.Client.Validators
{
    public class TechPowerValidator : AbstractValidator<TechPower>
    {
        public ISnackbar? Snackbar { get; set; }

        public TechPowerValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Level).InclusiveBetween(0, 10);
            RuleFor(x => x.Duration).NotEmpty();
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<TechPower>.CreateWithOptions((TechPower)model, x => x.IncludeProperties(propertyName)));
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