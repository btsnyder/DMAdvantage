using DMAdvantage.Client.Services;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Pages.EnemyShips
{
    public partial class EnemyShipEditForm
    {
        private EnemyShip _model = new();
        private MudForm _form;
        private bool _loading;
        private readonly ShipValidator _shipValidator = new();

        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        [Parameter] public string? Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            if (Id != null)
            {
                _model = await ApiService.GetEntityById<EnemyShip>(Guid.Parse(Id)) ?? new EnemyShip();
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
                    Snackbar.Add("Ship added successfully", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id), _model);
                    Snackbar.Add("Update successful", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
                NavigationManager.NavigateTo("enemyships");
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
            _shipValidator.Snackbar = Snackbar;
            await _form.Validate();
            _shipValidator.Snackbar = null;

            if (_form.IsValid)
            {
                OnValidSubmit();
            }
        }

        private void GenerateHullPoints()
        {
            var hull = _model.HullHitDice;
            hull += _model.ConstitutionBonus;
            for (int i = 1; i < _model.HullHitDiceNumber; i++)
            {
                hull += (_model.HullHitDice / 2) + 1;
                hull += _model.ConstitutionBonus;
            }
            _model.HullPoints = hull;
        }

        private void GenerateShieldPoints()
        {
            var shield = _model.ShieldHitDice;
            shield += _model.StrengthBonus;
            for (int i = 1; i < _model.ShieldHitDiceNumber; i++)
            {
                shield += (_model.ShieldHitDice / 2) + 1;
                shield += _model.StrengthBonus;
            }
            _model.ShieldPoints = shield;
        }
    }
}
