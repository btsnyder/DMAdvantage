using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using DMAdvantage.Client.Services;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.TechPowers
{
    public partial class TechIndex
    {
        private bool _loading;
        private readonly TechPowerSearchParameters _searching = new();
        private List<TechPowerResponse>? _techPowers;

        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await RefreshTechPowers();
        }

        private async Task RefreshTechPowers()
        {
            _techPowers = await ApiService.GetAllEntities<TechPowerResponse>(_searching);
            _loading = false;
        }

        private async Task RemoveTechPower(TechPowerResponse techPower)
        {
            if (_techPowers == null) return;
            _techPowers.Remove(techPower);
            await ApiService.RemoveEntity<TechPowerResponse>(techPower.Id);
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
            await RefreshTechPowers();
        }

        private void SetSearchEnum(IEnumerable<string> value, string property)
        {
            switch (property)
            {
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
