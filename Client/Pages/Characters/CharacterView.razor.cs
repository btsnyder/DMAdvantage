using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.Characters
{
    public partial class CharacterView
    {
        private IEnumerable<string> _weaponProperties = Array.Empty<string>();
        private CharacterResponse? _model = new();
        private bool _loading;
        private List<ForcePowerResponse> _forcePowers = new();

        [Inject] private IAlertService AlertService { get; set; }
        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        [Parameter] public string? PlayerName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _weaponProperties = Enum.GetNames<WeaponProperty>();
            if (PlayerName != null)
            {
                _model = await ApiService.GetCharacterViewFromPlayerName(PlayerName);
            }
            _forcePowers = await ApiService.GetViews<ForcePowerResponse>();

            await base.OnInitializedAsync();
        }

        private void OnValidSubmit()
        {
            _loading = true;
            try
            {
                NavigationManager.NavigateTo("characters");
            }
            catch (Exception ex)
            {
                AlertService.Alert(AlertType.Error, ex.Message);
                _loading = false;
                StateHasChanged();
            }
        }

        private static void WeaponPropertyChanged(Weapon weapon)
        {
            foreach (var prop in weapon.Properties)
            {
                var description = weapon.PropertyDescriptions.FirstOrDefault(x => x.Name == prop);
                if (description == null)
                {
                    weapon.PropertyDescriptions.Add(new WeaponDescription { Name = prop });
                }
            }
            weapon.PropertyDescriptions.RemoveAll(prop => weapon.PropertyDescriptions.Select(x => x.Name).Except(weapon.Properties).Contains(prop.Name));
        }

        private string CssStyle(IEntityResponse power)
        {
            var style = "fontsize: 14px;color: white;";
            if (_model?.ForcePowerIds.Contains(power.Id) == true)
                style += "background-color: #3888c2";
            else
                style += "background-color: #95a3ad";
            return style;
        }

        private void UpdateForcePower(IEntityResponse power)
        {
            if (_infoClicked || _model == null)
                return;
            if (_model.ForcePowerIds.Contains(power.Id))
            {
                _model.ForcePowerIds.Remove(power.Id);
            }
            else
            {
                _model.ForcePowerIds.Add(power.Id);
            }
        }

        private bool _infoClicked;

        private async Task InfoClicked(ForcePowerResponse power)
        {
            _infoClicked = true;
            await ShowInlineDialog(power);
            _infoClicked = false;
        }

        private string? GetPowerName(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return null;
            return _forcePowers.First(x => x.Id == id).Name;
        }

        private bool IsDisabled(ForcePowerResponse power)
        {
            if (_model!.ForcePowerIds.Contains(power.Id))
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
