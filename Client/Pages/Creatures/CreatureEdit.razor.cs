using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.Creatures
{
    public partial class CreatureEdit
    {
        private CreatureRequest? _model;
        private bool _loading;

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
            var creature = await ApiService.GetEntityById<CreatureResponse>(Guid.Parse(Id));
            _model = CustomMapper.Mapper.Map<CreatureRequest>(creature);

        }

        private async void OnValidSubmit()
        {
            _loading = true;
            try
            {
                await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                AlertService.Alert(AlertType.Success, "Update successful", keepAfterRouteChange: true);
                NavigationManager.NavigateTo("creatures");
            }
            catch (Exception ex)
            {
                AlertService.Alert(AlertType.Error, ex.Message);
                _loading = false;
                StateHasChanged();
            }
        }
    }
}
