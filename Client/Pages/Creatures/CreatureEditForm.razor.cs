using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.Creatures
{
    public partial class CreatureEditForm
    {
        IEnumerable<string> _damageTypes = Array.Empty<string>();
        IEnumerable<DamageType> _damageTypeEnums = Array.Empty<DamageType>();
        private CreatureRequest _model = new();
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
            _damageTypes = Enum.GetNames<DamageType>();
            _damageTypeEnums = Enum.GetValues<DamageType>();
            if (Id != null)
            {
                _model = await ApiService.GetEntityById<CreatureResponse>(Guid.Parse(Id)) ?? new();
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
                    AlertService.Alert(AlertType.Success, "Creature added successfully", keepAfterRouteChange: true);
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                    AlertService.Alert(AlertType.Success, "Update successful", keepAfterRouteChange: true);
                }
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
