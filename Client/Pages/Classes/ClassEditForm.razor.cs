using DMAdvantage.Client.Shared;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Entities;

namespace DMAdvantage.Client.Pages.Classes
{
    public partial class ClassEditForm : BaseEditForm<DMClass>
    {
        public ClassEditForm()
        {
            _validator = new ClassValidator();
        }
    }
}
