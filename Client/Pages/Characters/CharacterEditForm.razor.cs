using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
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
        private bool _loading;
        private List<ForcePowerResponse> _forcePowers = new();

        [Inject] private IAlertService AlertService { get; set; }
        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

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
                    AlertService.Alert(AlertType.Success, "Character added successfully", keepAfterRouteChange: true);
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                    AlertService.Alert(AlertType.Success, "Update successful", keepAfterRouteChange: true);
                }
                NavigationManager.NavigateTo("characters");
            }
            catch (Exception ex)
            {
                AlertService.Alert(AlertType.Error, ex.Message);
                _loading = false;
                StateHasChanged();
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
    }
}
