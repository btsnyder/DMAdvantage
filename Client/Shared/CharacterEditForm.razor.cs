using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Shared
{
    public partial class CharacterEditForm
    {
        IEnumerable<string> _weaponProperities = Array.Empty<string>();
        private CharacterRequest _model = new();
        private bool _loading;
        
        [Inject]
        IAlertService AlertService { get; set; }
        [Inject]
        IApiService ApiService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _weaponProperities = Enum.GetNames<WeaponProperty>();
            if (Id != null)
            {
                _model = await ApiService.GetEntityById<CharacterResponse>(Guid.Parse(Id)) ?? new();
            }
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
                    AlertService.Alert(AlertType.Success, "Character added successfully", keepAfterRouteChange: true);
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                    AlertService.Alert(AlertType.Success, "Update successful", keepAfterRouteChange: true);
                }
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
