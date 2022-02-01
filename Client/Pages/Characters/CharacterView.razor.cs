using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.Characters
{
    public partial class CharacterView
    {
        IEnumerable<string> _weaponProperities = Array.Empty<string>();
        private CharacterRequest? _model = new();
        private bool _loading;
        private List<ForcePowerResponse> _forcePowers = new();

        [Inject]
        IAlertService AlertService { get; set; }
        [Inject]
        IApiService ApiService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string? PlayerName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _weaponProperities = Enum.GetNames<WeaponProperty>();
            if (PlayerName != null)
            {
                _model = await ApiService.GetCharacterViewFromPlayerName(PlayerName);
            }
            _forcePowers = await ApiService.GetViews<ForcePowerResponse>() ?? new();

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

        void WeaponPropertyChanged(Weapon weapon)
        {
            foreach (var prop in weapon.Properties)
            {
                var description = weapon.PropertyDescriptions.FirstOrDefault(x => x.Name == prop);
                if (description == null)
                {
                    weapon.PropertyDescriptions.Add(new WeaponDescription { Name = prop });
                }
            }
            weapon.PropertyDescriptions.RemoveAll(x => weapon.PropertyDescriptions.Select(x => x.Name).Except(weapon.Properties).Contains(x.Name));
        }

        string CssStyle(ForcePowerResponse power)
        {
            var style = "fontsize: 14px;color: white;";
            if (_model.ForcePowerIds.Contains(power.Id))
                style += "background-color: #3888c2";
            else
                style += "background-color: #95a3ad";
            return style;
        }

        void UpdateForcePower(ForcePowerResponse power)
        {
            if (_infoClicked)
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
        async Task InfoClicked(ForcePowerResponse power)
        {
            _infoClicked = true;
            await ShowInlineDialog(power);
            _infoClicked = false;
        }

        string? GetPowerName(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return null;
            return _forcePowers.First(x => x.Id == id).Name;
        }

        bool IsDisabled(ForcePowerResponse power)
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
