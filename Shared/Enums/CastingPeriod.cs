namespace DMAdvantage.Shared.Enums
{
    public enum CastingPeriod
    {
        Action,
        [EnumString("Bonus Action")]
        BonusAction,
        Reaction,
        [EnumString("1 minute")]
        Minute,
        [EnumString("10 minutes")]
        TenMinutes,
        [EnumString("1 Hour")]
        Hour,
        [EnumString("8 Hours")]
        EightHours,
    }
}
