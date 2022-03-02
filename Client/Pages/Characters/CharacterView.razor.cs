using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.Characters
{
    public partial class CharacterView
    {
        private CharacterResponse? _model = new();
        private List<ForcePowerResponse>? _forcePowers = new();
        private bool _loading;

        [Inject] private IApiService ApiService { get; set; }
        [Parameter] public string? PlayerName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            if (PlayerName != null)
            {
                _model = await ApiService.GetCharacterViewFromPlayerName(PlayerName);
            }
            _forcePowers = await ApiService.GetViews<ForcePowerResponse>();

            await base.OnInitializedAsync();
            _loading = false;
        }

        private string? GetPowerName(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return null;
            return _forcePowers?.First(x => x.Id == id).Name;
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
