﻿using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.ForcePowers
{
    public partial class ForceEdit
    {
        private ForcePowerRequest? _model;
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
            var creature = await ApiService.GetEntityById<ForcePowerResponse>(Guid.Parse(Id));
            _model = CustomMapper.Mapper.Map<ForcePowerRequest>(creature);
        }

        private async void OnValidSubmit()
        {
            _loading = true;
            try
            {
                await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                AlertService.Alert(AlertType.Success, "Update successful", keepAfterRouteChange: true);
                NavigationManager.NavigateTo("forecepowers");
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
