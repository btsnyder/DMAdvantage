namespace DMAdvantage.Client.Helpers
{
    public static class DiceRoller
    {
        public static int Number { get; set; } = 1;
        public static int Sides { get; set; } = 4;
        public static int Bonus { get; set; }
        public static readonly List<int> Results = new();
        private static readonly Random _randomizer = new();

        public static int RollDice()
        {
            Results.Clear();
            for (int i = 0; i < Number; i++)
            {
                Results.Add(_randomizer.Next(1, Sides + 1));
            }
            return Results.Sum() + Bonus;
        }
    }
}
