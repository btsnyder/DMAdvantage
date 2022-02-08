using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace DMAdvantage.Client.Pages.TechPowers
{
    public partial class TechEditForm
    {
        private TechPowerRequest _model = new();
        private bool _loading;
        private List<TechPowerResponse> _techPowers;
        private readonly TechPowerSearchParameters _search = new();
        private IEnumerable<string> _durations = Array.Empty<string>();
        private List<string> _startingDurations = new();

        [Inject] private IAlertService AlertService { get; set; }
        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        [Parameter] public string? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _techPowers = await ApiService.GetAllEntities<TechPowerResponse>() ?? new List<TechPowerResponse>();
            _startingDurations = _techPowers.Select(x => x.Duration ?? string.Empty).Distinct().ToList();

            if (Id != null)
                _model = await ApiService.GetEntityById<TechPowerResponse>(Guid.Parse(Id)) ?? new TechPowerResponse();

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
                    AlertService.Alert(AlertType.Success, "Force power added successfully", true);
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                    AlertService.Alert(AlertType.Success, "Update successful", true);
                }

                NavigationManager.NavigateTo("techpowers");
            }
            catch (Exception ex)
            {
                AlertService.Alert(AlertType.Error, ex.Message);
                _loading = false;
                StateHasChanged();
            }
        }

        private void OnDurationChange(object value)
        {
            var duration = (string)value;
            _model.Duration = duration;
        }

        private async Task OnLoadDurationData(LoadDataArgs args)
        {
            var search = args.Filter;
            _model.Duration = search;
            _durations = _startingDurations.Where(x => x.ToLower().Contains(search.ToLower()));
            await InvokeAsync(StateHasChanged);
        }
    }
}