namespace GameResources.Features.PersistentProgress
{
    using System;

    [Serializable]
    public sealed class RankingEntryDto
    {
        public string PlayerId;
        public string AvatarAddress;
        public int Level;
        public string PlayerName;
        public int Score;
        public int LeagueRecord;
    }
}
