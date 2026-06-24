namespace GameResources.Features.UISystem
{
    using System.Collections.Generic;
    using PersistentProgress;
    using Signals;
    using UnityEngine;
    using UnityEngine.UI;
    using Object = UnityEngine.Object;

    public sealed class RankingWindow : BaseWindow
    {
        private const int TOP_RANK_COUNT = 3;
        private const string LEAGUE_ICON_ADDRESS_PREFIX = "League/";

        private IReadOnlyList<RankingEntryDto> _entries;
        private string _localPlayerId;
        private readonly AddressableSpriteCache _spriteCache = new();
        private readonly List<RankingItemView> _spawnedItems = new();
        private StickyPlayerController _sticky;

        [field: SerializeField] public Button CloseButton { get; private set; }

        [Header("Ranking")]
        [SerializeField] private RankingItemView _itemPrefab;
        [SerializeField] private Transform _itemParent;
        [SerializeField] private List<RectTransform> _layoutRoots = new();

        [Header("Sticky Player (optional)")]
        [SerializeField] private bool _enableStickyPlayer;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RankingItemView _stickyItem;

        private bool IsStickyActive => _enableStickyPlayer && _stickyItem != null && _scrollRect != null;

        private void Awake()
        {
            CloseButton.onClick.AddListener(OnCloseButtonClicked);
            if (IsStickyActive)
            {
                _sticky = new StickyPlayerController(_scrollRect, _stickyItem);
                _scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            }
        }

        private void OnDestroy()
        {
            CloseButton.onClick.RemoveListener(OnCloseButtonClicked);
            if (IsStickyActive)
            {
                _scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
            }
            _spriteCache.Dispose();
        }

        public void SetEntries(IReadOnlyList<RankingEntryDto> entries) => _entries = entries;

        public void SetLocalPlayerId(string playerId) => _localPlayerId = playerId;

        public override void Show()
        {
            base.Show();
            BuildLocalView();
            RebuildLayout();
            if (IsStickyActive)
            {
                _sticky.UpdateState();
            }
        }

        private void BuildLocalView()
        {
            ClearItems();
            _sticky?.Reset();
            if (_entries != null)
            {
                RectTransform playerRect = null;
                RankingEntryDto playerEntry = null;
                int playerRank = 0;
                for (int i = 0; i < _entries.Count; i++)
                {
                    RankingEntryDto entry = _entries[i];
                    int rank = i + 1;
                    bool isPlayer = IsLocalPlayer(entry);
                    RankingItemView item = Object.Instantiate(_itemPrefab, _itemParent);
                    item.UpdateView(rank, entry);
                    item.SetTopRankBackground(null);
                    item.SetHighlighted(isPlayer);
                    _spawnedItems.Add(item);
                    LoadAvatar(item, entry != null ? entry.AvatarAddress : null);
                    if (isPlayer)
                    {
                        playerRect = (RectTransform)item.transform;
                        playerEntry = entry;
                        playerRank = rank;
                    }
                }
                SetupSticky(playerRect, playerEntry, playerRank);
                LoadLeagueSprites();
            }
        }

        private int GetTopRankLeagueIndex(int rank) => TOP_RANK_COUNT - rank;

        private bool IsLocalPlayer(RankingEntryDto entry) => entry != null && !string.IsNullOrEmpty(_localPlayerId) && entry.PlayerId == _localPlayerId;

        private void LoadLeagueSprites()
        {
            HashSet<int> indices = new();
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i] != null)
                {
                    indices.Add(_entries[i].LeagueRecord);
                }
                int rank = i + 1;
                if (rank <= TOP_RANK_COUNT)
                {
                    indices.Add(GetTopRankLeagueIndex(rank));
                }
            }
            foreach (int index in indices)
            {
                LoadLeagueSprite(index);
            }
        }

        private void LoadLeagueSprite(int index) => _spriteCache.Load(LEAGUE_ICON_ADDRESS_PREFIX + index, sprite => ApplyLeagueSprite(index, sprite));

        private void ApplyLeagueSprite(int index, Sprite sprite)
        {
            for (int i = 0; i < _spawnedItems.Count; i++)
            {
                if (_spawnedItems[i] != null && _entries[i] != null)
                {
                    if (_entries[i].LeagueRecord == index)
                    {
                        _spawnedItems[i].SetLeagueIcon(sprite);
                    }
                    int rank = i + 1;
                    if (rank <= TOP_RANK_COUNT && GetTopRankLeagueIndex(rank) == index)
                    {
                        _spawnedItems[i].SetTopRankBackground(sprite);
                    }
                }
            }
            if (IsStickyActive && _sticky.Entry != null)
            {
                if (_sticky.Entry.LeagueRecord == index)
                {
                    _sticky.Item.SetLeagueIcon(sprite);
                }
                if (_sticky.Rank <= TOP_RANK_COUNT && GetTopRankLeagueIndex(_sticky.Rank) == index)
                {
                    _sticky.Item.SetTopRankBackground(sprite);
                }
            }
        }

        private void SetupSticky(RectTransform playerRect, RankingEntryDto playerEntry, int playerRank)
        {
            if (IsStickyActive && playerRect != null && playerEntry != null)
            {
                _sticky.Setup(playerRect, playerEntry, playerRank);
                LoadAvatar(_sticky.Item, playerEntry.AvatarAddress);
            }
        }

        private void OnScrollValueChanged(Vector2 _) => _sticky.UpdateState();

        private void LoadAvatar(RankingItemView item, string avatarAddress)
        {
            if (!string.IsNullOrWhiteSpace(avatarAddress))
            {
                _spriteCache.Load(avatarAddress, sprite =>
                {
                    if (item != null)
                    {
                        item.SetAvatar(sprite);
                    }
                });
            }
        }

        private void ClearItems()
        {
            for (int i = 0; i < _spawnedItems.Count; i++)
            {
                if (_spawnedItems[i] != null)
                {
                    Object.Destroy(_spawnedItems[i].gameObject);
                }
            }
            _spawnedItems.Clear();
        }

        private void RebuildLayout()
        {
            for (int i = 0; i < _layoutRoots.Count; i++)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutRoots[i]);
            }
        }

        private void OnCloseButtonClicked() => _signalBus.Fire<RankingCloseRequestedSignal>();
    }
}
