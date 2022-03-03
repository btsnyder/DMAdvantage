using DMAdvantage.Client.Services;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Pages.Abilities
{
    public partial class AbilityEditForm
    {
        private AbilityRequest _model = new();
        private MudForm _form;
        private bool _loading;
        private readonly AbilityRequestFluentValidator _abilityValidator = new();

        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        [Parameter] public string? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            if (Id != null)
            {
                _model = await ApiService.GetEntityById<AbilityResponse>(Guid.Parse(Id)) ?? new AbilityResponse();
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
                    Snackbar.Add("Ability added successfully", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                    Snackbar.Add("Update successful", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                NavigationManager.NavigateTo("abilities");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error submitting change: {ex}", Severity.Error);
            }
            _loading = false;
        }

        private async Task OnSubmit()
        {
            _abilityValidator.Snackbar = Snackbar;
            await _form.Validate();
            _abilityValidator.Snackbar = null;

            if (_form.IsValid)
            {
                OnValidSubmit();
            }
        }
    }
}
