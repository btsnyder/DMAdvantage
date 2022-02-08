using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.TechPowers
{
    public partial class TechIndex
    {
        private bool _loading;
        private IEnumerable<int>? _selectedLevels;
        private IEnumerable<string>? _selectedCastingPeriods;
        private IEnumerable<string>? _selectedPowerRanges;
        private readonly IEnumerable<int> _levels = new [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private readonly IEnumerable<string> _castingPeriods = Enum.GetValues<CastingPeriod>().Select(x => x.GetStringValue());
        private readonly IEnumerable<string> _powerRanges = Enum.GetValues<PowerRange>().Select(x => x.GetStringValue());
        private CancellationTokenSource? _source;
        private CancellationToken _token;
        public async Task SearchChanged(ChangeEventArgs e)
        {
            await RefreshTechPowers();
        }

        private readonly PagingParameters _paging = new();
        private readonly TechPowerSearchParameters _searching = new();
        private PagedList<TechPowerResponse>? _techPowers;

        protected override async Task OnInitializedAsync()
        {
            await RefreshTechPowers();
        }

        private async Task RefreshTechPowers()
        {
            _loading = true;
            _source?.Cancel();
            _source = new CancellationTokenSource();
            _token = _source.Token;

            _searching.Levels = _selectedLevels?.ToArray() ?? Array.Empty<int>();
            _searching.CastingPeriods = _selectedCastingPeriods == null ? Array.Empty<CastingPeriod>() :
                EnumExtensions.GetEnumValues<CastingPeriod>(_selectedCastingPeriods).ToArray();
            _searching.Ranges = _selectedPowerRanges == null ? Array.Empty<PowerRange>() :
                EnumExtensions.GetEnumValues<PowerRange>(_selectedPowerRanges).ToArray();
            _techPowers = await ApiService.GetAllPagedEntities<TechPowerResponse>(_paging, _searching, _token);
            _loading = false;
        }

        private async Task RemoveTechPower(TechPowerResponse techPower)
        {
            if (_techPowers == null)
                return;
            _techPowers.Remove(techPower);
            await ApiService.RemoveEntity<TechPowerResponse>(techPower.Id);
        }

        private async Task CurrentPageChanged(int page)
        {
            _paging.PageNumber = page;
            await RefreshTechPowers();
        }
    }
}
