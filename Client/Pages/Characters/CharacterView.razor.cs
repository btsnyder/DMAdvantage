using DMAdvantage.Client.Helpers;
using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.Characters
{
    public partial class CharacterView
    {
        private CharacterResponse? _model = new();
        private List<ForcePowerResponse>? _forcePowers = new();
        private bool _loading;
        private string _skillSearch;
        private readonly string[] _strSkills = new string[] { "StrengthSave", "Athletics" };
        private readonly string[] _dexSkills = new string[] { "DexteritySave", "Acrobatics", "SleightOfHand", "Stealth" };
        private readonly string[] _conSkills = new string[] { "ConstitutionSave" };
        private readonly string[] _intSkills = new string[] { "IntelligenceSave", "Investigation", "Lore", "Nature", "Piloting", "Technology" };
        private readonly string[] _wisSkills = new string[] { "WisdomSave", "AnimalHandling", "Insight", "Medicine", "Perception", "Survival" };
        private readonly string[] _chaSkills = new string[] { "CharismaSave", "Deception", "Intimidation", "Performance", "Persuasion" };
        private IEnumerable<string> _startingSkills => _strSkills.Union(_dexSkills).Union(_conSkills).Union(_intSkills).Union(_wisSkills).Union(_chaSkills).OrderBy(x => x);
        private IEnumerable<string> _filteredSkills;

        [Inject] private IApiService ApiService { get; set; }
        [Parameter] public string? PlayerName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            if (PlayerName != null)
            {
                _model = await ApiService.GetCharacterViewFromPlayerName(PlayerName);
            }
            _forcePowers = await ApiService.GetViews<ForcePowerResponse>();
            _filteredSkills = _startingSkills;

            await base.OnInitializedAsync();
            _loading = false;
        }

        private string? GetPowerName(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return null;
            return _forcePowers?.First(x => x.Id == id).Name;
        }

        private bool IsDisabled(ForcePowerResponse power)
        {
            if (_model!.ForcePowers.Contains(power))
                return false;
            if (_model.ForcePowers.Count >= _model.TotalForcePowers)
                return true;
            if (power.PrerequisiteId.HasValue && power.PrerequisiteId != Guid.Empty)
            {
                return !_model.ForcePowers.Any(f => f.Id == power.PrerequisiteId.Value);
            }
            return false;
        }

        private string GetSkillBonus(string name)
        {
            if (_model == null) return string.Empty;
            var proficiency = (bool?)typeof(CharacterResponse).GetProperty(name)?.GetValue(_model);
            if (_strSkills.Contains(name))
                return _model.SkillBonus(_model.StrengthBonus, proficiency).PrintInt();
            if (_dexSkills.Contains(name))
                return _model.SkillBonus(_model.DexterityBonus, proficiency).PrintInt();
            if (_conSkills.Contains(name))
                return _model.SkillBonus(_model.ConstitutionBonus, proficiency).PrintInt();
            if (_intSkills.Contains(name))
                return _model.SkillBonus(_model.IntelligenceBonus, proficiency).PrintInt();
            if (_wisSkills.Contains(name))
                return _model.SkillBonus(_model.WisdomBonus, proficiency).PrintInt();
            if (_chaSkills.Contains(name))
                return _model.SkillBonus(_model.CharismaBonus, proficiency).PrintInt();
            return string.Empty;
        }

        private void SkillSearch(string val)
        {
            _filteredSkills = _startingSkills.Where(x => x.ToLower().Contains(val.ToLower()));
        }
    }
}
