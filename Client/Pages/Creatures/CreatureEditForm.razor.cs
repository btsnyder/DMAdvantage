using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Client.Services.Implementations;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Pages.Creatures
{
    public partial class CreatureEditForm
    {
        IEnumerable<string> _damageTypes = Array.Empty<string>();
        private CreatureRequest _model = new();
        private bool _loading;
        private MudForm _form;
        private readonly CreatureRequestFluentValidator _creatureValidator = new();

        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }


        [Parameter] public string? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _damageTypes = Enum.GetNames<DamageType>();
            if (Id != null)
            {
                _model = await ApiService.GetEntityById<CreatureResponse>(Guid.Parse(Id)) ?? new CreatureResponse();
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
                    Snackbar.Add("Creature added successfully", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                    Snackbar.Add("Update successful", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                NavigationManager.NavigateTo("creatures");
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
            _creatureValidator.Snackbar = Snackbar;
            await _form.Validate();
            _creatureValidator.Snackbar = null;

            if (_form.IsValid)
            {
                OnValidSubmit();
            }
        }

        private void SelectedValuesChanged(IEnumerable<string> val, string prop)
        {
            switch (prop)
            {
                case nameof(_model.Vulnerabilities):
                    _model.Vulnerabilities = EnumExtensions.GetEnumValues<DamageType>(val);
                    break;
                case nameof(_model.Immunities):
                    _model.Immunities = val;
                    break;
                case nameof(_model.Resistances):
                    _model.Resistances = val;
                    break;
                default:
                    throw new NotImplementedException($"Unknown value type in CreateEditForm: {prop}");
            }
        }
    }
}
