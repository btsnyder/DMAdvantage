using DMAdvantage.Client.Validators;

namespace DMAdvantage.Client.Pages.ShipWeaponProperties
{
    public partial class ShipWeaponPropertyEditForm
    {
        public ShipWeaponPropertyEditForm()
        {
            _validator = new ShipWeaponPropertyValidator();
        }
    }
}
