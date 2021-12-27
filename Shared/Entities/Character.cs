﻿using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Entities
{
    public class Character : Being
    {
        public string? PlayerName { get; set; }
        public int Level { get; set; }
        public SWClass? Class { get; set; }

        public override string OrderBy()
        {
            return Name ?? string.Empty;
        }
    }
}
