using DMAdvantage.Shared.Entities;

namespace DMAdvantage.Shared.Extensions
{
    public static class AbilityExtensions
    {
        public static void StrengthChanged(this IAbilityEntity entity, string value)
        {
            if (!int.TryParse(value, out var str)) return;
            entity.Strength = str;
            entity.StrengthBonus = CalculateBonus(str);
        }

        public static void DexterityChanged(this IAbilityEntity entity, string value)
        {
            if (!int.TryParse(value, out var dex)) return;
            entity.Dexterity = dex;
            entity.DexterityBonus = CalculateBonus(dex);
        }

        public static void ConstitutionChanged(this IAbilityEntity entity, string value)
        {
            if (!int.TryParse(value, out var con)) return;
            entity.Constitution = con;
            entity.ConstitutionBonus = CalculateBonus(con);
        }

        public static void IntelligenceChanged(this IAbilityEntity entity, string value)
        {
            if (!int.TryParse(value, out var intelligence)) return;
            entity.Intelligence = intelligence;
            entity.IntelligenceBonus = CalculateBonus(intelligence);
        }

        public static void WisdomChanged(this IAbilityEntity entity, string value)
        {
            if (!int.TryParse(value, out var wis)) return;
            entity.Wisdom = wis;
            entity.WisdomBonus = CalculateBonus(wis);
        }

        public static void CharismaChanged(this IAbilityEntity entity, string value)
        {
            if (!int.TryParse(value, out var cha)) return;
            entity.Charisma = cha;
            entity.CharismaBonus = CalculateBonus(cha);
        }

        private static int CalculateBonus(int value)
        {
            var bonus = value / 2;
            return bonus - 5;
        }
    }
}
