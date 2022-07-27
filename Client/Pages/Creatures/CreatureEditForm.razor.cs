using DMAdvantage.Client.Shared;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Extensions;

namespace DMAdvantage.Client.Pages.Creatures
{
    public partial class CreatureEditForm : BaseEditForm<Creature>
    {
        IEnumerable<string> _damageTypes = Array.Empty<string>();

        public CreatureEditForm()
        {
            _validator = new CreatureValidator();
        }

        protected override async Task OnInitializedAsync()
        {
            _damageTypes = Enum.GetNames<DamageType>();
            await base.OnInitializedAsync();
        }

        private void SelectedValuesChanged(IEnumerable<string> val, string prop)
        {
            switch (prop)
            {
                case nameof(_model.Vulnerabilities):
                    _model.Vulnerabilities = EnumExtensions.GetEnumValues<DamageType>(val).ToList();
                    break;
                case nameof(_model.Immunities):
                    _model.Immunities = EnumExtensions.GetEnumValues<DamageType>(val).ToList();
                    break;
                case nameof(_model.Resistances):
                    _model.Resistances = EnumExtensions.GetEnumValues<DamageType>(val).ToList();
                    break;
                default:
                    throw new NotImplementedException($"Unknown value type in CreateEditForm: {prop}");
            }
        }
    }
}
