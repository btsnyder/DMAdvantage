namespace DMAdvantage.Shared.Models
{
    public static class BeingModelExtensions
    {
        public static void StrengthChanged(this BeingModel being, string value)
        {
            if (!int.TryParse(value, out var str)) return;
            being.Strength = str;
            being.StrengthBonus = CalculateBonus(str);
        }

        public static void DexterityChanged(this BeingModel being, string value)
        {
            if (!int.TryParse(value, out var dex)) return;
            being.Dexterity = dex;
            being.DexterityBonus = CalculateBonus(dex);
        }

        public static void ConstitutionChanged(this BeingModel being, string value)
        {
            if (!int.TryParse(value, out var con)) return;
            being.Constitution = con;
            being.ConstitutionBonus = CalculateBonus(con);
        }

        public static void IntelligenceChanged(this BeingModel being, string value)
        {
            if (!int.TryParse(value, out var intelligence)) return;
            being.Intelligence = intelligence;
            being.IntelligenceBonus = CalculateBonus(intelligence);
        }

        public static void WisdomChanged(this BeingModel being, string value)
        {
            if (!int.TryParse(value, out var wis)) return;
            being.Wisdom = wis;
            being.WisdomBonus = CalculateBonus(wis);
        }

        public static void CharismaChanged(this BeingModel being, string value)
        {
            if (!int.TryParse(value, out var cha)) return;
            being.Charisma = cha;
            being.CharismaBonus = CalculateBonus(cha);
        }

        private static int CalculateBonus(int value)
        {
            var bonus = value / 2;
            return bonus - 5;
        }
    }
}
