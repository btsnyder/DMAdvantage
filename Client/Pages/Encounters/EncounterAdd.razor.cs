using DMAdvantage.Client.Models;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace DMAdvantage.Client.Pages.Encounters
{
    public partial class EncounterAdd
    {
        private readonly EncounterRequest _model = new();
        private bool _loading;
        private readonly List<InitativeDataModel> _initatives = new();
        private List<CharacterResponse> _characters;
        private List<CreatureResponse> _creatures;
        private CharacterResponse _selectedCharacter;
        private CreatureResponse _selectedCreature;
        private int _healthEdit;
        private bool _initativeEditing;

        [Inject]
        IAlertService AlertService { get; set; }
        [Inject]
        IApiService ApiService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _characters = await ApiService.GetAllEntities<CharacterResponse>() ?? new();
            _creatures = await ApiService.GetAllEntities<CreatureResponse>() ?? new();

            await base.OnInitializedAsync();
        }

        private async void OnValidSubmit()
        {
            _loading = true;
            try
            {
                foreach (var init in _initatives)
                {
                    _model.Data.Add(init);
                }
                await ApiService.AddEntity(_model);
                AlertService.Alert(AlertType.Success, "Character added successfully", keepAfterRouteChange: true);
                NavigationManager.NavigateTo("encounters");
            }
            catch (Exception ex)
            {
                AlertService.Alert(AlertType.Error, ex.Message);
                _loading = false;
                StateHasChanged();
            }
        }

        void ConfirmHealth(InitativeDataModel data)
        {
            data.ApplyHP(_healthEdit);
            data.Healing = data.Damaging = false;
            _healthEdit = 0;
        }

        void CancelHealth(InitativeDataModel data)
        {
            data.Healing = data.Damaging = false;
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

        void OnAddCharacter()
        {
            var data = new InitativeDataModel(_selectedCharacter);
            _initatives.Add(data);
        }

        void OnAddCreature()
        {
            var data = new InitativeDataModel(_selectedCreature);
            _initatives.Add(data);
        }

        void InitativeEditDone()
        {
            _initativeEditing = false;
            _initatives.Sort(delegate (InitativeDataModel data1, InitativeDataModel data2) { return data2.Initative.CompareTo(data1.Initative); });
        }
    }
}
