namespace DMAdvantage.Shared.Extensions
{
    public static class IntExtensions
    {
        public static string PrintInt(this int i)
        {
            if (i >= 0)
                return $"+{i}";
            else
                return i.ToString();
        }
    }
}
