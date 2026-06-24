namespace GameResources.Features.UISystem
{
    using PersistentProgress;
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class StickyPlayerController
    {
        private const float VISIBILITY_EPSILON = 1f;

        private readonly ScrollRect _scrollRect;
        private readonly RankingItemView _stickyItem;
        private readonly Vector3[] _worldCorners = new Vector3[4];

        private RectTransform _playerRect;
        private RankingEntryDto _entry;
        private int _rank;

        public StickyPlayerController(ScrollRect scrollRect, RankingItemView stickyItem)
        {
            _scrollRect = scrollRect;
            _stickyItem = stickyItem;
        }

        public RankingItemView Item => _stickyItem;
        public RankingEntryDto Entry => _entry;
        public int Rank => _rank;

        public void Reset()
        {
            _playerRect = null;
            _entry = null;
            _rank = 0;
            _stickyItem.gameObject.SetActive(false);
        }

        public void Setup(RectTransform playerRect, RankingEntryDto entry, int rank)
        {
            _playerRect = playerRect;
            _entry = entry;
            _rank = rank;
            _stickyItem.UpdateView(rank, entry);
            _stickyItem.SetHighlighted(true);
            _stickyItem.SetTopRankBackground(null);
            UpdateState();
        }

        public void UpdateState()
        {
            if (_playerRect != null)
            {
                Rect viewportRect = GetWorldRect((RectTransform)_scrollRect.viewport);
                Rect playerWorldRect = GetWorldRect(_playerRect);
                if (playerWorldRect.yMax > viewportRect.yMax + VISIBILITY_EPSILON)
                {
                    Pin(true);
                }
                else if (playerWorldRect.yMin < viewportRect.yMin - VISIBILITY_EPSILON)
                {
                    Pin(false);
                }
                else
                {
                    _stickyItem.gameObject.SetActive(false);
                }
            }
        }

        private void Pin(bool toTop)
        {
            RectTransform stickyRect = (RectTransform)_stickyItem.transform;
            Vector2 anchor = toTop ? new Vector2(0.5f, 1f) : new Vector2(0.5f, 0f);
            stickyRect.anchorMin = anchor;
            stickyRect.anchorMax = anchor;
            stickyRect.pivot = anchor;
            stickyRect.anchoredPosition = Vector2.zero;
            _stickyItem.gameObject.SetActive(true);
        }

        private Rect GetWorldRect(RectTransform rectTransform)
        {
            rectTransform.GetWorldCorners(_worldCorners);
            float xMin = _worldCorners[0].x;
            float yMin = _worldCorners[0].y;
            float xMax = _worldCorners[2].x;
            float yMax = _worldCorners[2].y;
            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }
    }
}
