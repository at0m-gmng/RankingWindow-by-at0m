namespace GameResources.Features.UISystem
{
    using PersistentProgress;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class RankingItemView : MonoBehaviour
    {
        private const int LEVEL_UNKNOWN = 0;
        private const string SCORE_FORMAT = "N0";
        private const string LEVEL_UNKNOWN_LABEL = "-";

        [Header("Texts")]
        [SerializeField] private TMP_Text _rankText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _levelText;

        [Header("Avatar")]
        [SerializeField] private Image _avatarImage;
        [SerializeField] private GameObject _avatarObject;
        [SerializeField] private GameObject _fakeAvatarObject;

        [Header("League")]
        [SerializeField] private Image _leagueIcon;

        [Header("Top Rank")]
        [SerializeField] private Image _topRankBackground;

        [Header("Local Player Highlight")]
        [SerializeField] private Image _highlightBackground;
        [SerializeField] private Color _highlightColor = new Color(1f, 0.85f, 0.3f, 0.35f);

        public void UpdateView(int rank, RankingEntryDto dto)
        {
            if (dto == null)
            {
                UpdateView(rank, string.Empty, 0, LEVEL_UNKNOWN);
            }
            else
            {
                UpdateView(rank, dto.PlayerName, dto.Score, dto.Level);
            }
        }

        public void UpdateView(int rank, string playerName, int score, int level)
        {
            _rankText.text = rank.ToString();
            _nameText.text = playerName;
            _scoreText.text = score.ToString(SCORE_FORMAT);
            _levelText.text = level == LEVEL_UNKNOWN ? LEVEL_UNKNOWN_LABEL : level.ToString();
            ShowFakeAvatar();
        }

        public void SetAvatar(Sprite sprite)
        {
            if (sprite == null)
            {
                ShowFakeAvatar();
            }
            else
            {
                _avatarImage.sprite = sprite;
                SetAvatarActive(true);
            }
        }

        public void SetLeagueIcon(Sprite sprite)
        {
            if (sprite == null)
            {
                _leagueIcon.enabled = false;
            }
            else
            {
                _leagueIcon.sprite = sprite;
                _leagueIcon.enabled = true;
            }
        }

        public void SetTopRankBackground(Sprite sprite)
        {
            if (sprite == null)
            {
                _topRankBackground.gameObject.SetActive(false);
            }
            else
            {
                _topRankBackground.sprite = sprite;
                _topRankBackground.gameObject.SetActive(true);
            }
        }

        public void SetHighlighted(bool isLocalPlayer)
        {
            if (isLocalPlayer)
            {
                _highlightBackground.color = _highlightColor;
                _highlightBackground.enabled = true;
            }
            else
            {
                _highlightBackground.enabled = false;
            }
        }

        private void ShowFakeAvatar() => SetAvatarActive(false);

        private void SetAvatarActive(bool hasRealAvatar)
        {
            _avatarObject.SetActive(hasRealAvatar);
            _fakeAvatarObject.SetActive(!hasRealAvatar);
        }
    }
}
