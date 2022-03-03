using DMAdvantage.Shared.Models;
using FluentValidation;
using MudBlazor;

namespace DMAdvantage.Client.Validators
{
    public class AbilityRequestFluentValidator : AbstractValidator<AbilityRequest>
    {
        public ISnackbar? Snackbar { get; set; }

        public AbilityRequestFluentValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<AbilityRequest>.CreateWithOptions((AbilityRequest)model, x => x.IncludeProperties(propertyName)));
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