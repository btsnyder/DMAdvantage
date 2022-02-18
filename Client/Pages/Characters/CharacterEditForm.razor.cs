using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace DMAdvantage.Client.Pages.Characters
{
    public partial class CharacterEditForm
    {
        private IEnumerable<string> _weaponProperties = Array.Empty<string>();
        private CharacterRequest _model = new();
        private bool _loading;
        private List<ForcePowerResponse> _forcePowers = new();
        private MudForm _form;
        private readonly CharacterRequestFluentValidator _characterValidator = new();

        [Inject] private IAlertService AlertService { get; set; }
        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        [Parameter]
        public string? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _weaponProperties = Enum.GetNames<WeaponProperty>();
            if (Id != null)
            {
                _model = await ApiService.GetEntityById<CharacterResponse>(Guid.Parse(Id)) ?? new CharacterResponse();
            }
            _forcePowers = await ApiService.GetAllEntities<ForcePowerResponse>() ?? new List<ForcePowerResponse>();

            await base.OnInitializedAsync();
        }

        private async void OnValidSubmit()
        {
            _loading = true;
            try
            {
                if (Id == null)
                {
                    await ApiService.AddEntity(_model);
                    Snackbar.Add("Character added successfully", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                    Snackbar.Add("Update successful", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                NavigationManager.NavigateTo("characters");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error submitting change: {ex}", Severity.Error);
                _loading = false;
                StateHasChanged();
            }
        }

        private async Task OnSubmit()
        {
            _characterValidator.Snackbar = Snackbar;
            await _form.Validate();
            _characterValidator.Snackbar = null;

            if (_form.IsValid)
            {
                OnValidSubmit();
            }
        }

        private Color ForceColor(ForcePowerResponse power)
        {
            return _model.ForcePowerIds.Contains(power.Id) ? Color.Primary : Color.Default;
        }

        private string ForceInfoColor(ForcePowerResponse power)
        {
            return _model.ForcePowerIds.Contains(power.Id) ? Colors.Shades.White : Colors.Shades.Black;
        }

        private void UpdateForcePower(ForcePowerResponse power)
        {
            if (_model.ForcePowerIds.Contains(power.Id))
            {
                _model.ForcePowerIds.Remove(power.Id);
            }
            else
            {
                _model.ForcePowerIds.Add(power.Id);
            }
        }

        private string? GetPowerName(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return null;
            return _forcePowers.First(x => x.Id == id).Name;
        }

        private bool IsDisabled(ForcePowerResponse power)
        {
            if (_model.ForcePowerIds.Contains(power.Id))
                return false;
            if (_model.ForcePowerIds.Count >= _model.TotalForcePowers)
                return true;
            if (power.PrerequisiteId.HasValue && power.PrerequisiteId != Guid.Empty)
            {
                return !_model.ForcePowerIds.Contains(power.PrerequisiteId.Value);
            }
            return false;
        }

        private class CharacterRequestFluentValidator : AbstractValidator<CharacterRequest>
        {
            public ISnackbar? Snackbar { get; set; }

            public CharacterRequestFluentValidator()
            {
                RuleFor(x => x.Name).NotEmpty();
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
                var result = await ValidateAsync(ValidationContext<CharacterRequest>.CreateWithOptions((CharacterRequest)model, x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                    return Array.Empty<string>();
                var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                if (Snackbar == null) return errors;
                foreach (var error in errors)
                {
                    Snackbar.Add(error, Severity.Error);
                }
                return errors;
            };
        }
    }
}
