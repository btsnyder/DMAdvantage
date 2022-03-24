using DMAdvantage.Client.Services;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Pages.Classes
{
    public partial class ClassEditForm
    {
        private DMClass _model = new();
        private MudForm _form;
        private bool _loading;
        private readonly ClassValidator _classValidator = new();

        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        [Parameter] public string? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            if (Id != null)
            {
                _model = await ApiService.GetEntityById<DMClass>(Guid.Parse(Id)) ?? new DMClass();
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
                    Snackbar.Add("Class added successfully", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                    Snackbar.Add("Update successful", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                NavigationManager.NavigateTo("classes");
            }
            catch
            {
                Snackbar.Add($"Error submitting change!", Severity.Error);
            }
            _loading = false;
            StateHasChanged();
        }

        private async Task OnSubmit()
        {
            _classValidator.Snackbar = Snackbar;
            await _form.Validate();
            _classValidator.Snackbar = null;

            if (_form.IsValid)
            {
                OnValidSubmit();
            }
        }
    }
}
