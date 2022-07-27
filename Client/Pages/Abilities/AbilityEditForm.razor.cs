using DMAdvantage.Client.Validators;
using DMAdvantage.Client.Shared;
using DMAdvantage.Shared.Entities;

namespace DMAdvantage.Client.Pages.Abilities
{
    public partial class AbilityEditForm : BaseEditForm<Ability>
    {
        public AbilityEditForm()
        {
            _validator = new AbilityValidator();
        }
    }
}
