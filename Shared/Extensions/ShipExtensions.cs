using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Extensions;

namespace DMAdvantage.Shared.Models
{
    public static class ShipExtensions
    {
        public static int SkillBonus(this Ship ship, int bonus, bool? proficient)
        {
            return proficient switch
            {
                false => bonus,
                null => bonus + 2,
                _ => bonus + 2 * 2
            };
        }

        public static string WeaponBonus(this Ship ship, ShipWeapon weapon)
        {
            if (weapon.Type == ShipWeaponType.Primary || weapon.Type == ShipWeaponType.Secondary)
                return ship.StrengthBonus.PrintInt();
            return string.Empty;
        }
    }
}
