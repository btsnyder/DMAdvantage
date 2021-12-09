using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.Creatures
{
    public partial class CreatureAdd
    {
        private readonly CreatureRequest _model = new();
        private bool _loading;

        [Inject]
        IAlertService AlertService { get; set; }
        [Inject]
        IApiService ApiService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        private async void OnValidSubmit()
        {
            _loading = true;
            try
            {
                await ApiService.AddEntity(_model);
                AlertService.Alert(AlertType.Success, "Creature added successfully", keepAfterRouteChange: true);
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
