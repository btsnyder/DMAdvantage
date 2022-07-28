using DMAdvantage.Client.Shared;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Entities;

namespace DMAdvantage.Client.Pages.EnemyShips
{
    public partial class EnemyShipEditForm : BaseEditForm<EnemyShip>
    {
        public EnemyShipEditForm()
        {
            _validator = new EnemyShipValidator();
        }
        private void GenerateHullPoints()
        {
            var hull = _model.HullHitDice;
            hull += _model.ConstitutionBonus;
            for (int i = 1; i < _model.HullHitDiceNumber; i++)
            {
                hull += (_model.HullHitDice / 2) + 1;
                hull += _model.ConstitutionBonus;
            }
            _model.HullPoints = hull;
        }

        private void GenerateShieldPoints()
        {
            var shield = _model.ShieldHitDice;
            shield += _model.StrengthBonus;
            for (int i = 1; i < _model.ShieldHitDiceNumber; i++)
            {
                shield += (_model.ShieldHitDice / 2) + 1;
                shield += _model.StrengthBonus;
            }
            _model.ShieldPoints = shield;
        }
    }
}
