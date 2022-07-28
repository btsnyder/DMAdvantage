using DMAdvantage.Client.Shared;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Entities;

namespace DMAdvantage.Client.Pages.ForcePowers
{
    public partial class ForceEditForm : BaseEditForm<ForcePower>
    {
        private List<ForcePower> _forcePowers;
        private ForcePower? _selectedPrerequisite;
        private List<string> _startingDurations = new();

        public ForceEditForm()
        {
            _validator = new ForcePowerValidator();
        }

        protected override async Task OnInitializedAsync()
        {
            _forcePowers = await ApiService.GetAllEntities<ForcePower>() ?? new List<ForcePower>();
            _startingDurations = _forcePowers.Select(x => x.Duration ?? string.Empty).Distinct().ToList();

            await base.OnInitializedAsync();

            if (_model.PrerequisiteId != null)
            {
                var prereq = await ApiService.GetEntityById<ForcePower>(_model.PrerequisiteId.Value);
                _selectedPrerequisite = prereq;
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
            return Task.FromResult(_startingDurations
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
