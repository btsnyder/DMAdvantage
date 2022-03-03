using DMAdvantage.Client.Services;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Pages.Characters
{
    public partial class CharacterEditForm
    {
        private IEnumerable<string> _weaponProperties = Array.Empty<string>();
        private CharacterRequest _model = new();
        private List<ForcePowerResponse> _forcePowers = new();
        private MudForm _form;
        private bool _loading;
        private readonly CharacterRequestFluentValidator _characterValidator = new();

        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        [Parameter] public string? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            _weaponProperties = Enum.GetNames<WeaponProperty>();
            if (Id != null)
            {
                _model = await ApiService.GetEntityById<CharacterResponse>(Guid.Parse(Id)) ?? new CharacterResponse();
            }
            _forcePowers = await ApiService.GetAllEntities<ForcePowerResponse>() ?? new List<ForcePowerResponse>();

            await base.OnInitializedAsync();
            _loading = false;
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
                Snackbar.Add($"Error submitting change: {ex.Message}", Severity.Error);
            }
            _loading = false;
            StateHasChanged();
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
            return _model.ForcePowerIds.Contains(power.Id) ? Color.Primary : Color.Dark;
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
    }
}
