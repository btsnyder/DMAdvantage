using DMAdvantage.Client.Validators;

namespace DMAdvantage.Client.Pages.ShipWeapons
{
    public partial class ShipWeaponEditForm
    {
        public ShipWeaponEditForm()
        {
            _validator = new ShipWeaponValidator();
        }
    }
}
