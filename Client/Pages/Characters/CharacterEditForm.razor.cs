using DMAdvantage.Client.Services;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Pages.Characters
{
    public partial class CharacterEditForm
    {
        private Character _model = new();
        private List<ForcePower> _forcePowers = new();
        private List<DMClass> _classes = new();
        private List<WeaponProperty> _weaponProperties = new();
        private MudForm _form;
        private bool _loading;
        private readonly CharacterValidator _characterValidator = new();

        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        [Parameter] public string? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            if (Id != null)
            {
                _model = await ApiService.GetEntityById<Character>(Guid.Parse(Id)) ?? new Character();
            }
            _forcePowers = await ApiService.GetAllEntities<ForcePower>() ?? new List<ForcePower>();
            _classes = await ApiService.GetAllEntities<DMClass>() ?? new List<DMClass>();
            _weaponProperties = await ApiService.GetAllEntities<WeaponProperty>() ?? new List<WeaponProperty>();

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
            catch
            {
                Snackbar.Add($"Error submitting change!", Severity.Error);
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

        private Color ForceColor(ForcePower power)
        {
            return _model.ForcePowers.Contains(power) ? Color.Primary : Color.Dark;
        }

        private void UpdateForcePower(ForcePower power)
        {
            if (_model.ForcePowers.Contains(power))
            {
                _model.ForcePowers.Remove(power);
            }
            else
            {
                _model.ForcePowers.Add(power);
            }
        }

        private string? GetPowerName(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return null;
            return _forcePowers.First(x => x.Id == id).Name;
        }

        private bool IsDisabled(ForcePower power)
        {
            if (_model.ForcePowers.Contains(power))
                return false;
            if (_model.ForcePowers.Count >= _model.TotalForcePowers)
                return true;
            if (power.PrerequisiteId.HasValue && power.PrerequisiteId != Guid.Empty)
            {
                return !_model.ForcePowers.Any(f => f.Id == power.PrerequisiteId.Value);
            }
            return false;
        }
    }
}
