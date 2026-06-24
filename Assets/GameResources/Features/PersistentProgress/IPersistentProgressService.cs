namespace GameResources.Features.PersistentProgress
{
    using System.Collections.Generic;

    public interface IPersistentProgressService : IPersistentListener
    {
        public void InitNewProgress();
        public bool TryLoadData();
        public void SaveData();
    }

    public interface IPersistentListener
    {
        public string PlayerId { get; }
        public int Score { get; }
        public int ScoreRecord { get; }
        public IReadOnlyList<RankingEntryDto> Ranking { get; }
    }
}
