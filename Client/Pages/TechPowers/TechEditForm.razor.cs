using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Pages.TechPowers
{
    public partial class TechEditForm
    {
        private TechPowerRequest _model = new();
        private bool _loading;
        private List<TechPowerResponse> _techPowers;
        private IEnumerable<string> _durations = Array.Empty<string>();
        private List<string> _startingDurations = new();
        private MudForm _form;
        private readonly TechPowerRequestFluentValidator _techPowerValidator = new();

        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        [Parameter] public string? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _techPowers = await ApiService.GetAllEntities<TechPowerResponse>() ?? new List<TechPowerResponse>();
            _startingDurations = _techPowers.Select(x => x.Duration ?? string.Empty).Distinct().ToList();

            if (Id != null)
                _model = await ApiService.GetEntityById<TechPowerResponse>(Guid.Parse(Id)) ?? new TechPowerResponse();

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
                    Snackbar.Add("Tech power added successfully", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                    Snackbar.Add("Update successful", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }

                NavigationManager.NavigateTo("techpowers");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error submitting change: {ex}", Severity.Error);
                _loading = false;
                StateHasChanged();
            }
        }

        private async Task OnSubmit()
        {
            _techPowerValidator.Snackbar = Snackbar;
            await _form.Validate();
            _techPowerValidator.Snackbar = null;

            if (_form.IsValid)
            {
                OnValidSubmit();
            }
        }

        private Task<IEnumerable<string>> DurationSearch(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return Task.FromResult<IEnumerable<string>>(Array.Empty<string>());
            return Task.FromResult(_durations
                .Where(x => x.ToLower().Contains(value.ToLower())));
        }
    }
}