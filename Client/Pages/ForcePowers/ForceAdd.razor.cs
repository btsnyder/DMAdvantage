﻿using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.ForcePowers
{
    public partial class ForceAdd
    {
        private readonly ForcePowerRequest _model = new();
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
    }
}