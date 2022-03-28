﻿using DMAdvantage.Client.Services;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Pages.WeaponProperties
{
    public partial class WeaponPropertyEditForm
    {
        private WeaponProperty _model = new();
        private MudForm _form;
        private bool _loading;
        private readonly WeaponPropertyValidator _weaponPropertyValidator = new();

        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        [Parameter] public string? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            if (Id != null)
            {
                _model = await ApiService.GetEntityById<WeaponProperty>(Guid.Parse(Id)) ?? new WeaponProperty();
            }

            await base.OnInitializedAsync();
            _loading = false;
        }

        private async void OnValidSubmit()
        {
            _loading = true;
            try
            {
                if (Id == null)
                {
                    await ApiService.AddEntity(_model);
                    Snackbar.Add("Weapon Property added successfully", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                    Snackbar.Add("Update successful", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                NavigationManager.NavigateTo("weaponproperties");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error submitting change: {ex}", Severity.Error);
            }
            _loading = false;
        }

        private async Task OnSubmit()
        {
            _weaponPropertyValidator.Snackbar = Snackbar;
            await _form.Validate();
            _weaponPropertyValidator.Snackbar = null;

            if (_form.IsValid)
            {
                OnValidSubmit();
            }
        }
    }
}
