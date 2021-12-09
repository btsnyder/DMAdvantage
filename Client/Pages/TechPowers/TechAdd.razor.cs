using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.TechPowers
{
    public partial class TechAdd
    {
        private readonly TechPowerRequest _model = new();
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
                AlertService.Alert(AlertType.Success, "Tech Power added successfully", keepAfterRouteChange: true);
                NavigationManager.NavigateTo("techpowers");
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
