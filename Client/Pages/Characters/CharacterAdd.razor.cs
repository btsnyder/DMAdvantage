using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.Characters
{
    public partial class CharacterAdd
    {
        private readonly CharacterRequest _model = new();
        private bool _loading;

        [Inject]
        IAlertService AlertService { get; set; }
        [Inject]
        IApiService ApiService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        private async void OnValidSubmit()
        {
            _loading = true;
            try
            {
                await ApiService.AddEntity(_model);
                AlertService.Alert(AlertType.Success, "Character added successfully", keepAfterRouteChange: true);
                NavigationManager.NavigateTo("characters");
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
