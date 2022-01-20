using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace DMAdvantage.Client.Pages.Encounters
{
    public partial class EncounterEditForm
    {
        private bool _loading;
        private List<InitativeDataModel> _initatives = new();
        private RadzenDataGrid<InitativeDataModel> _initativeGrid;
        private IList<InitativeDataModel> SelectedInitatives { get; set; }
        private InitativeDataModel? SelectedInitative => SelectedInitatives?.FirstOrDefault();
        private InitativeDataModel? _currentPlayer;
        IList<ForcePowerResponse> SelectedForcePowers { get; set; } = new List<ForcePowerResponse>();
        private List<CharacterResponse> _characters;
        private List<CreatureResponse> _creatures;
        private List<ForcePowerResponse> _forcePowers;
        private CharacterResponse _selectedCharacter;
        private CreatureResponse _selectedCreature;
        private int _healthEdit;
        private bool _initativeEditing;
        private Dictionary<string, ForcePowerResponse> _concentrationPowers = new();

        private EncounterRequest _model = new();

        [Parameter]
        public string? Id { get; set; }

        [Inject]
        IAlertService AlertService { get; set; }
        [Inject]
        IApiService ApiService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (Id != null)
            {
                _model = await ApiService.GetEntityById<EncounterResponse>(Guid.Parse(Id)) ?? new();

                List<IBeingResponse> beings = new();
                var characters = await ApiService.GetCharacterViews(_model.Data.Select(x => x.BeingId));
                var creatures = await ApiService.GetCreatureViews(_model.Data.Select(x => x.BeingId));
                beings.AddRange(characters);
                beings.AddRange(creatures);

                foreach (var being in beings)
                {
                    _initatives.Add(new InitativeDataModel(being, _model.Data.First(x => x.BeingId == being.Id)));
                }

                var sorted = new List<InitativeDataModel>(_initatives);
                sorted.Sort(delegate (InitativeDataModel data1, InitativeDataModel data2) { return data2.Initative.CompareTo(data1.Initative); });
                _initatives = new(sorted);
            }

            SelectedInitatives = new List<InitativeDataModel>() { _initatives[0] };

            _characters = await ApiService.GetAllEntities<CharacterResponse>() ?? new();
            _creatures = await ApiService.GetAllEntities<CreatureResponse>() ?? new();
            _forcePowers = await ApiService.GetAllEntities<ForcePowerResponse>() ?? new();

            if (SelectedInitative != null)
                OnRowSelect(SelectedInitative);

            _currentPlayer = _initatives.FirstOrDefault(x => x.BeingId == _model.CurrentPlayer);

            foreach (var concentration in _model.ConcentrationPowers)
            {
                var power = _forcePowers.FirstOrDefault(x => x.Id == concentration.Value);
                if (power != null)
                    _concentrationPowers.Add(concentration.Key, power);
            }

            await base.OnInitializedAsync();
        }

        private async void OnValidSubmit()
        {
            _loading = true;
            try
            {
                _model.Data.Clear();
                foreach (var init in _initatives)
                {
                    _model.Data.Add(init);
                }
                _model.CurrentPlayer = _currentPlayer?.BeingId ?? Guid.Empty;
                _model.ConcentrationPowers.Clear();
                foreach (var power in _concentrationPowers)
                {
                    _model.ConcentrationPowers.Add(power.Key, power.Value.Id);
                }
                if (Id == null)
                {
                    await ApiService.AddEntity(_model);
                    AlertService.Alert(AlertType.Success, "Character added successfully", keepAfterRouteChange: true);
                    NavigationManager.NavigateTo("encounters");
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id ?? string.Empty), _model);
                    AlertService.Alert(AlertType.Success, "Update successful", keepAfterRouteChange: true);
                }
            }
            catch (Exception ex)
            {
                AlertService.Alert(AlertType.Error, ex.Message);
                _loading = false;
                StateHasChanged();
            }
            finally
            {
                _loading = false;
                StateHasChanged();
            }
        }

        void ApplyHealth()
        {
            SelectedInitative?.ApplyHP(_healthEdit);
            _healthEdit = 0;
        }

        void ApplyDamage()
        {
            SelectedInitative?.ApplyHP(-1 * _healthEdit);
            _healthEdit = 0;
        }

        void OnCharacterChange(object value)
        {
            var name = (string)value;
            var character = _characters.FirstOrDefault(x => x.Name == name);
            if (character != null)
                _selectedCharacter = character;
        }


        void OnCreatureChange(object value)
        {
            var name = (string)value;
            var creature = _creatures.FirstOrDefault(x => x.Name == name);
            if (creature != null)
                _selectedCreature = creature;
        }

        void OnRowSelect(InitativeDataModel data)
        {
            if (data.Being != null)
            {
                SelectedForcePowers = _forcePowers.Where(x => data.Being.ForcePowerIds.Contains(x.Id)).OrderBy(x => x.Level).ThenBy(x => x.Name).ToList();
            }
        }

        async Task OnLoadCharacterData(LoadDataArgs args)
        {
            var characterSearch = new NamedSearchParameters<Character> { Search = args.Filter };
            _characters = await ApiService.GetAllEntities<CharacterResponse>(characterSearch) ?? new();

            await InvokeAsync(StateHasChanged);
        }

        async Task OnLoadCreatureData(LoadDataArgs args)
        {
            var creatureSearch = new NamedSearchParameters<Creature> { Search = args.Filter };
            _creatures = await ApiService.GetAllEntities<CreatureResponse>(creatureSearch) ?? new();

            await InvokeAsync(StateHasChanged);
        }

        async Task OnAddCharacter()
        {
            var data = new InitativeDataModel(_selectedCharacter);
            _initatives.Insert(0, data);
            await _initativeGrid.InsertRow(data);
            await _initativeGrid.UpdateRow(data);
        }

        async Task OnAddCreature()
        {
            var data = new InitativeDataModel(_selectedCreature);
            _initatives.Insert(0, data);
            await _initativeGrid.InsertRow(data);
            await _initativeGrid.UpdateRow(data);
        }

        async Task InitativeEditStart()
        {
            _initativeEditing = true;
            foreach (var row in _initatives)
            {
                await _initativeGrid.EditRow(row);
            }
        }

        async Task InitativeEditDone()
        {
            _initativeEditing = false;
            foreach (var row in _initatives)
            {
                await _initativeGrid.UpdateRow(row);
            }
            var sorted = new List<InitativeDataModel>(_initatives);
            sorted.Sort(delegate (InitativeDataModel data1, InitativeDataModel data2) { return data2.Initative.CompareTo(data1.Initative); });
            _initatives = new(sorted); 
        }

        void InitativeNext()
        {
            int index;
            if (_currentPlayer == null || _initatives.Last() == _currentPlayer)
                index = -1;
            else
                index = _initatives.IndexOf(_currentPlayer);
            _currentPlayer = _initatives[++index];
        }

        void InitativePrevious()
        {
            int index;
            if (_currentPlayer == null || _initatives.First() == _currentPlayer)
                index = _initatives.Count;
            else
                index = _initatives.IndexOf(_currentPlayer);
            _currentPlayer = _initatives[--index];
        }

        async Task ForcePowerClicked(ForcePowerResponse? power)
        {
            if (SelectedInitative == null || power == null || power.Level < 1)
                return;
            if (SelectedInitative.CurrentFP < power.Level + 1)
            {
                #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                InvokeAsync(async () =>
                {
                    await Task.Delay(1500);
                    DialogService.Close();
                });
                #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                await BusyDialog($"Not enough force points!");
            }
            else
            {
                SelectedInitative.CurrentFP -= power.Level + 1;
                if (power.Concentration)
                {
                    _concentrationPowers.Add(SelectedInitative.Name ?? $"Player {_initatives.IndexOf(SelectedInitative)}", power);
                }
                StateHasChanged();
            }
        }
    }
}
