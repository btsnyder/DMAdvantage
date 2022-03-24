using DMAdvantage.Client.Services;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Pages.ForcePowers
{
    public partial class ForceEditForm
    {
        private ForcePower _model = new();
        private bool _loading;
        private List<ForcePower> _forcePowers;
        private ForcePower? _selectedPrerequisite;
        private IEnumerable<string> _durations = Array.Empty<string>();
        private List<string> _startingDurations = new();
        private MudForm _form;
        private readonly ForcePowerValidator _forcePowerValidator = new();

        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        [Parameter] public string? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _forcePowers = await ApiService.GetAllEntities<ForcePower>() ?? new List<ForcePower>();
            _startingDurations = _forcePowers.Select(x => x.Duration ?? string.Empty).Distinct().ToList();

            if (Id != null)
                _model = await ApiService.GetEntityById<ForcePower>(Guid.Parse(Id)) ?? new ForcePower();
            
            if (_model.PrerequisiteId != null)
            {
                var prereq = await ApiService.GetEntityById<ForcePower>(_model.PrerequisiteId.Value);
                _selectedPrerequisite = prereq;
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
                    Snackbar.Add("Force power added successfully", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                    Snackbar.Add("Update successful", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                NavigationManager.NavigateTo("forcepowers");
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
            _forcePowerValidator.Snackbar = Snackbar;
            await _form.Validate();
            _forcePowerValidator.Snackbar = null;

            if (_form.IsValid)
            {
                OnValidSubmit();
            }
        }

        private void OnPrerequisiteChange(ForcePower? value)
        {
            _selectedPrerequisite = value;
            _model.PrerequisiteId = value?.Id;
        }

        private Task<IEnumerable<string>> DurationSearch(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return Task.FromResult<IEnumerable<string>>(Array.Empty<string>());
            return Task.FromResult(_durations
                .Where(x => x.ToLower().Contains(value.ToLower())));
        }

        private Task<IEnumerable<ForcePower?>> PrerequisiteSearch(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return Task.FromResult<IEnumerable<ForcePower?>>(Array.Empty<ForcePower>());
            return Task.FromResult<IEnumerable<ForcePower?>>(_forcePowers
                .Where(x => x.Name?.ToLower().Contains(value.ToLower()) == true));
        }
    }
}
