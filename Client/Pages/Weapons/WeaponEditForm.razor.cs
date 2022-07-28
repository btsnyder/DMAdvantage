using DMAdvantage.Client.Validators;

namespace DMAdvantage.Client.Pages.Weapons
{
    public partial class WeaponEditForm
    {
        public WeaponEditForm()
        {
            _validator = new WeaponValidator();
        }
    }
}
