namespace DMAdvantage.Shared.Models
{
    public static class BeingModelExtensions
    {
        public static void StrengthChanged(this BeingModel being, int value)
        {
            being.Strength = value;
            being.StrengthBonus = CalculateBonus(value);
        }

        public static void DexterityChanged(this BeingModel being, int value)
        {
            being.Dexterity = value;
            being.DexterityBonus = CalculateBonus(value);
        }

        public static void ConstitutionChanged(this BeingModel being, int value)
        {
            being.Constitution = value;
            being.ConstitutionBonus = CalculateBonus(value);
        }

        public static void IntelligenceChanged(this BeingModel being, int value)
        {
            being.Intelligence = value;
            being.IntelligenceBonus = CalculateBonus(value);
        }

        public static void WisdomChanged(this BeingModel being, int value)
        {
            being.Wisdom = value;
            being.WisdomBonus = CalculateBonus(value);
        }

        public static void CharismaChanged(this BeingModel being, int value)
        {
            being.Charisma = value;
            being.CharismaBonus = CalculateBonus(value);
        }

        private static int CalculateBonus(int value)
        {
            var bonus = value / 2;
            return bonus - 5;
        }
    }
}
