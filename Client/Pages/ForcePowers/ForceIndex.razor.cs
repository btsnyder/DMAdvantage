using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.ForcePowers
{
    public partial class ForceIndex
    {
        private bool _loading;
        private IEnumerable<int>? _selectedLevels;
        private IEnumerable<string>? _selectedAlignments;
        private IEnumerable<string>? _selectedCastingPeriods;
        private IEnumerable<string>? _selectedPowerRanges;
        private readonly IEnumerable<int> _levels = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private readonly IEnumerable<string> _alignments = Enum.GetValues<ForceAlignment>().Select(x => x.GetStringValue());
        private readonly IEnumerable<string> _castingPeriods = Enum.GetValues<CastingPeriod>().Select(x => x.GetStringValue());
        private readonly IEnumerable<string> _powerRanges = Enum.GetValues<PowerRange>().Select(x => x.GetStringValue());
        private CancellationTokenSource _source;
        private CancellationToken _token;
        public async Task SearchChanged(ChangeEventArgs e)
        {
            await RefreshForcePowers();
        }

        private readonly PagingParameters _paging = new();
        private readonly ForcePowerSearchParameters _searching = new();
        private PagedList<ForcePowerResponse>? _forcePowers;

        protected override async Task OnInitializedAsync()
        {
            await RefreshForcePowers();
        }

        private async Task RefreshForcePowers()
        {
            _loading = true;
            _source?.Cancel();
            _source = new CancellationTokenSource();
            _token = _source.Token;

            _searching.Levels = _selectedLevels?.ToArray() ?? Array.Empty<int>();
            _searching.Alignments = _selectedAlignments == null ? Array.Empty<ForceAlignment>() : 
                EnumExtensions.GetEnumValues<ForceAlignment>(_selectedAlignments).ToArray();
            _searching.CastingPeriods = _selectedCastingPeriods == null ? Array.Empty<CastingPeriod>() : 
                EnumExtensions.GetEnumValues<CastingPeriod>(_selectedCastingPeriods).ToArray();
            _searching.Ranges = _selectedPowerRanges == null ? Array.Empty<PowerRange>() :
                EnumExtensions.GetEnumValues<PowerRange>(_selectedPowerRanges).ToArray();
            _forcePowers = await ApiService.GetAllPagedEntities<ForcePowerResponse>(_paging, _searching, _token);
            _loading = false;
        }

        private async Task RemoveForcePower(ForcePowerResponse forcePower)
        {
            if (_forcePowers == null)
                return;
            _forcePowers.Remove(forcePower);
            await ApiService.RemoveEntity<ForcePowerResponse>(forcePower.Id);
        }

        private async Task CurrentPageChanged(int page)
        {
            _paging.PageNumber = page;
            await RefreshForcePowers();
        }
    }
}
