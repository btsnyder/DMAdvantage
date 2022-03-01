using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.ForcePowers
{
    public partial class ForceIndex
    {
        private bool _loading;
        private readonly ForcePowerSearchParameters _searching = new();
        private List<ForcePowerResponse>? _forcePowers;

        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await RefreshForcePowers();
        }

        private async Task RefreshForcePowers()
        {
            _forcePowers = await ApiService.GetAllEntities<ForcePowerResponse>(_searching);
            _loading = false;
        }

        private async Task RemoveForcePower(ForcePowerResponse forcePower)
        {
            if (_forcePowers == null) return;
            _forcePowers.Remove(forcePower);
            await ApiService.RemoveEntity<ForcePowerResponse>(forcePower.Id);
        }

        public async Task SearchChanged<T>(T value, string property)
        {
            switch (value)
            {
                case string search:
                    _searching.Search = search;
                    break;
                case IEnumerable<int> levels:
                    _searching.Levels = levels.ToArray();
                    break;
                case IEnumerable<string> selected:
                    SetSearchEnum(selected, property);
                    break;
                case IEnumerable<CastingPeriod> castingPeriods:
                    _searching.CastingPeriods = castingPeriods.ToArray();
                    break;
                case IEnumerable<PowerRange> powerRanges:
                    _searching.Ranges = powerRanges.ToArray();
                    break;
                default:
                    throw new ArgumentException("Not a handled search type");
            }
            await RefreshForcePowers();
        }

        private void SetSearchEnum(IEnumerable<string> value, string property)
        {
            switch (property)
            {
                case nameof(ForcePowerSearchParameters.Alignments):
                    _searching.Alignments = EnumExtensions.GetEnumValues<ForceAlignment>(value).ToArray();
                    break;
                case nameof(ForcePowerSearchParameters.CastingPeriods):
                    _searching.CastingPeriods = EnumExtensions.GetEnumValues<CastingPeriod>(value).ToArray();
                    break;
                case nameof(ForcePowerSearchParameters.Ranges):
                    _searching.Ranges = EnumExtensions.GetEnumValues<PowerRange>(value).ToArray();
                    break;
                default:
                    throw new ArgumentException("Not a handled search type");
            }
        }
    }
}
