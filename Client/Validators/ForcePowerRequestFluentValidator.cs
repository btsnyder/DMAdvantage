using DMAdvantage.Shared.Models;
using FluentValidation;
using MudBlazor;

namespace DMAdvantage.Client.Validators
{
    public class ForcePowerRequestFluentValidator : AbstractValidator<ForcePowerRequest>
    {
        public ISnackbar? Snackbar { get; set; }

        public ForcePowerRequestFluentValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Level).InclusiveBetween(0, 10);
            RuleFor(x => x.Duration).NotEmpty();
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ForcePowerRequest>.CreateWithOptions((ForcePowerRequest)model, x => x.IncludeProperties(propertyName)));
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