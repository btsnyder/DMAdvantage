using DMAdvantage.Client.Validators;

namespace DMAdvantage.Client.Pages.WeaponProperties
{
    public partial class WeaponPropertyEditForm
    {
        public WeaponPropertyEditForm()
        {
            _validator = new WeaponPropertyValidator();
        }
    }
}
