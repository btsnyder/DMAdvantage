using DMAdvantage.Client.Services;
using DMAdvantage.Shared;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;

namespace DMAdvantage.Client.Pages.Encounters
{
    public partial class EncounterView
    {
        private EncounterResponse? _model;
        private bool _loading;
        private List<InitativeDataModel> _initatives = new();
        private InitativeDataModel? _currentPlayer;
        private List<ForcePowerResponse> _forcePowers;
        private IList<ForcePowerResponse> SelectedForcePowers { get; set; } = new List<ForcePowerResponse>();
        private Dictionary<string, ForcePowerResponse> _concentrationPowers = new();
        public bool _autoLoad;
        public Timer _timer;
        private readonly MudTheme _theme = new();


        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IDialogService DialogService { get; set;}
        [Parameter] public string Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _forcePowers = await ApiService.GetViews<ForcePowerResponse>();

            NavigationManager.LocationChanged += NavigationManager_LocationChanged;

            await ReloadEncounter();
            await base.OnInitializedAsync();
        }

        private void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
        {
            _timer.Dispose();
            NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                _timer = new Timer(async _ =>
                {
                    if (_autoLoad)
                    {
                        await ReloadEncounter();
                    }
                }, null, 3000, 3000);
            }
        }

        private async Task ReloadEncounter()
        {
            _model = await ApiService.GetEncounterView(Guid.Parse(Id));

            if (_model != null)
            {
                List<IBeingResponse> beings = new();
                var characters = await ApiService.GetViews<CharacterResponse>(_model.Data.Select(x => x.BeingId));
                var creatures = await ApiService.GetViews<CreatureResponse>(_model.Data.Select(x => x.BeingId));
                beings.AddRange(characters);
                beings.AddRange(creatures);

                _initatives.Clear();
                foreach (var being in beings)
                {
                    _initatives.Add(new InitativeDataModel(being, _model.Data.First(x => x.BeingId == being.Id)));
                }

                var sorted = new List<InitativeDataModel>(_initatives);
                sorted.Sort((data1, data2) => data2.Initative.CompareTo(data1.Initative));
                _initatives = new List<InitativeDataModel>(sorted);

                _currentPlayer = _initatives.FirstOrDefault(x => x.BeingId == _model.CurrentPlayer);
                if (_currentPlayer?.Being != null)
                {
                    SelectedForcePowers = _forcePowers.Where(x => _currentPlayer.Being.ForcePowerIds.Contains(x.Id))
                        .OrderBy(x => x.Level).ThenBy(x => x.Name).ToList();
                }

                _concentrationPowers.Clear();
                foreach (var (key, value) in _model.ConcentrationPowers)
                {
                    var power = _forcePowers.FirstOrDefault(x => x.Id == value);
                    if (power != null)
                        _concentrationPowers.Add(key, power);
                }
            }

            StateHasChanged();
        }

        private string GetRowClass(InitativeDataModel data)
        {
            return data == _currentPlayer ? "indigo lighten-4" : string.Empty;
        }

        private string HealthBackground(InitativeDataModel data)
        {
            return data == _currentPlayer ? Colors.Indigo.Lighten4 : string.Empty;
        }

        private void ShowWeaponProperty(WeaponDescription weapon)
        {
            var description = StaticData.WeaponPropertyDescriptions[Enum.Parse<WeaponProperty>(weapon.Name ?? string.Empty)];
            DialogService.ShowMessageBox(weapon.Name, description);
        }
    }
}
