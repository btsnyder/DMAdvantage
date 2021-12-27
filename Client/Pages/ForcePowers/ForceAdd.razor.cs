using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace DMAdvantage.Client.Pages.ForcePowers
{
    public partial class ForceAdd
    {
        private readonly ForcePowerRequest _model = new();
        private bool _loading;
        private List<ForcePowerResponse> _forcePowers;
        private readonly ForcePowerSearchParameters _search = new();
        private IEnumerable<string> _durations = Array.Empty<string>();
        private List<string> _startingDurations = new();

        [Inject]
        IAlertService AlertService { get; set; }
        [Inject]
        IApiService ApiService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _forcePowers = await ApiService.GetAllEntities<ForcePowerResponse>() ?? new();
            _startingDurations = _forcePowers.Select(x => x.Duration ?? string.Empty).Distinct().ToList();

            await base.OnInitializedAsync();
        }

        private async void OnValidSubmit()
        {
            _loading = true;
            try
            {
                await ApiService.AddEntity(_model);
                AlertService.Alert(AlertType.Success, "Force Power added successfully", keepAfterRouteChange: true);
                NavigationManager.NavigateTo("forcepowers");
            }
            catch (Exception ex)
            {
                AlertService.Alert(AlertType.Error, ex.Message);
                _loading = false;
                StateHasChanged();
            }
        }

        void OnChange(object value)
        {
            var name = (string)value;
            var power = _forcePowers.FirstOrDefault(x => x.Name == name);
            if (power != null)
                _model.PrerequisiteId = power.Id;
            else
                _model.PrerequisiteId = null;
        }

        async Task OnLoadData(LoadDataArgs args)
        {
            _search.Search = args.Filter;
            _forcePowers = await ApiService.GetAllEntities<ForcePowerResponse>(_search) ?? new();

            await InvokeAsync(StateHasChanged);
        }

        void OnDurationChange(object value)
        {
            var duration = (string)value;
            _model.Duration = duration;
        }

        async Task OnLoadDurationData(LoadDataArgs args)
        {
            var search = args.Filter;
            _model.Duration = search;
            _durations = _startingDurations.Where(x => x.ToLower().Contains(search.ToLower()));
            await InvokeAsync(StateHasChanged);
        }
    }
}
