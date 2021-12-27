using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace DMAdvantage.Client.Pages.ForcePowers
{
    public partial class ForceEdit
    {
        private ForcePowerRequest? _model;
        private bool _loading;
        private List<ForcePowerResponse> _forcePowers;
        private string? _selectedPrerequisite;
        private readonly ForcePowerSearchParameters _search = new();

        [Inject]
        IAlertService AlertService { get; set; }
        [Inject]
        IApiService ApiService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Parameter]
        public string Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _forcePowers = await ApiService.GetAllEntities<ForcePowerResponse>() ?? new();

            _model = await ApiService.GetEntityById<ForcePowerResponse>(Guid.Parse(Id));
            if (_model?.PrerequisiteId != null)
            {
                var prereq = await ApiService.GetEntityById<ForcePowerResponse>(_model.PrerequisiteId.Value);
                _selectedPrerequisite = prereq?.Name;
            }
            
            await base.OnInitializedAsync();
        }

        private async void OnValidSubmit()
        {
            _loading = true;
            try
            {
                await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                AlertService.Alert(AlertType.Success, "Update successful", keepAfterRouteChange: true);
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
            if (_model == null)
                return;
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
    }
}
