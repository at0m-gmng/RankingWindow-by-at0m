using System;
using System.Collections.Generic;
using GameResources.Features.SaveLoadSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameResources.Features.PersistentProgress
{
    public sealed class PersistentProgressService : IPersistentProgressService, IDisposable
    {
        private const string RANKING_DATA_ADDRESS = "RankingData";
        private const string LOCAL_PLAYER_ID = "uid_13";

        public PersistentProgressService(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }
        private readonly ISaveLoadService _saveLoadService;

        private RankingDataDto _rankingData = new RankingDataDto { Entries = Array.Empty<RankingEntryDto>() };
        private AsyncOperationHandle<TextAsset> _rankingHandle;

        public string PlayerId => LOCAL_PLAYER_ID;
        public int Score { get; private set; }
        public int ScoreRecord { get; private set; }
        public IReadOnlyList<RankingEntryDto> Ranking => _rankingData.Entries;

        public void Dispose()
        {
            if (_rankingHandle.IsValid())
            {
                Addressables.Release(_rankingHandle);
            }
        }

        public void InitNewProgress() => _rankingData = new RankingDataDto { Entries = Array.Empty<RankingEntryDto>() };

        public bool TryLoadData()
        {
            bool loaded = false;
            _rankingHandle = Addressables.LoadAssetAsync<TextAsset>(RANKING_DATA_ADDRESS);
            TextAsset jsonAsset = _rankingHandle.WaitForCompletion();
            if (jsonAsset != null && !string.IsNullOrWhiteSpace(jsonAsset.text))
            {
                _rankingData = JsonUtility.FromJson<RankingDataDto>(jsonAsset.text);
                loaded = _rankingData != null && _rankingData.Entries != null;
            }
            return loaded;
        }

        public void SaveData()
        {
        }
    }
}
