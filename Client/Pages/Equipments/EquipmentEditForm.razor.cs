using DMAdvantage.Client.Shared;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Entities;

namespace DMAdvantage.Client.Pages.Equipments
{
    public partial class EquipmentEditForm : BaseEditForm<Equipment>
    {
        public EquipmentEditForm()
        {
            _validator = new EquipmentValidator();
        }
    }
}
