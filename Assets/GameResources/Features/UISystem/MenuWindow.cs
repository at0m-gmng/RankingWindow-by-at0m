namespace GameResources.Features.UISystem
{
    using DG.Tweening;
    using Signals;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class MenuWindow : BaseWindow
    {
        private const int MAX_PLACEMENT_ATTEMPTS = 24;

        [Header("Buttons")]
        [field: SerializeField] public Button ButtonStart { get; private set; } = default;

        [Header("Start Button Animation")]
        [SerializeField] private float _pressScale = 0.8f;
        [SerializeField] private float _pressDuration = 0.25f;
        [SerializeField] private float _releaseScale = 1.2f;
        [SerializeField] private float _releaseDuration = 1f;

        [Header("Runaway Button")]
        [SerializeField] private bool _enableRunaway = true;
        [SerializeField] private TMP_Text _buttonText;
        [SerializeField] private string _finishedText = "Ну держи!";
        [SerializeField] private int _escapeCount = 3;
        [SerializeField] private float _escapeMargin = 24f;

        private Sequence _buttonAnimation = null;
        private RectTransform _buttonRect;
        private RectTransform _moveArea;
        private Vector2 _originalAnchoredPosition;
        private string _originalText;
        private int _escapesLeft;
        private bool _runawayFinished;
        private readonly Vector3[] _worldCorners = new Vector3[4];

        private void Awake()
        {
            CreateButtonAnimation();
            ButtonStart.onClick.AddListener(OnStartButtonClicked);
            if (_enableRunaway)
            {
                InitRunaway();
            }
        }

        private void OnDestroy()
        {
            ButtonStart.onClick.RemoveListener(OnStartButtonClicked);
            _buttonAnimation.Kill();
        }

        private void OnStartButtonClicked() => _signalBus.Fire<MenuStartRequestedSignal>();

        private void Update()
        {
            if (_enableRunaway && !_runawayFinished)
            {
                Vector2 cursorWorldPoint = GetCursorWorldPoint();
                if (IsCursorOverButton(cursorWorldPoint))
                {
                    if (_escapesLeft > 0)
                    {
                        MoveAwayFrom(cursorWorldPoint);
                        _escapesLeft--;
                    }
                    else
                    {
                        FinishRunaway();
                    }
                }
            }
        }

        public override void Show()
        {
            base.Show();
            _buttonAnimation.Restart();
            if (_enableRunaway)
            {
                ResetRunaway();
            }
        }

        public override void Hide()
        {
            _buttonAnimation.Rewind();
            base.Hide();
        }

        private void CreateButtonAnimation()
        {
            _buttonAnimation = DOTween.Sequence();
            _buttonAnimation
                .Append(ButtonStart.transform.DOScale(_pressScale, _pressDuration).SetEase(Ease.Linear))
                .Append(ButtonStart.transform.DOScale(_releaseScale, _releaseDuration).SetEase(Ease.OutBack));
            _buttonAnimation.SetAutoKill(false).SetLoops(-1, LoopType.Yoyo);
        }

        private void InitRunaway()
        {
            _buttonRect = (RectTransform)ButtonStart.transform;
            _moveArea = (RectTransform)_buttonRect.parent;
            _originalAnchoredPosition = _buttonRect.anchoredPosition;
            _originalText = _buttonText.text;
            _escapesLeft = _escapeCount;
        }

        private void ResetRunaway()
        {
            _runawayFinished = false;
            _escapesLeft = _escapeCount;
            _buttonRect.anchoredPosition = _originalAnchoredPosition;
            _buttonText.text = _originalText;
        }

        private Vector2 GetCursorWorldPoint()
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_moveArea, Input.mousePosition, null, out Vector3 worldPoint);
            return worldPoint;
        }

        private Vector2 GetButtonWorldHalfSize()
        {
            Vector2 size = _buttonRect.rect.size;
            Vector3 areaScale = _moveArea.lossyScale;
            return new Vector2(size.x * areaScale.x, size.y * areaScale.y) * 0.5f;
        }

        private bool IsCursorOverButton(Vector2 cursorWorldPoint)
        {
            Vector2 half = GetButtonWorldHalfSize();
            Vector2 center = _buttonRect.position;
            return Mathf.Abs(cursorWorldPoint.x - center.x) <= half.x && Mathf.Abs(cursorWorldPoint.y - center.y) <= half.y;
        }

        private void MoveAwayFrom(Vector2 cursorWorldPoint)
        {
            Vector2 half = GetButtonWorldHalfSize();
            _moveArea.GetWorldCorners(_worldCorners);
            float minX = _worldCorners[0].x + half.x;
            float maxX = _worldCorners[2].x - half.x;
            float minY = _worldCorners[0].y + half.y;
            float maxY = _worldCorners[2].y - half.y;
            Vector3 target = _buttonRect.position;
            for (int attempt = 0; attempt < MAX_PLACEMENT_ATTEMPTS; attempt++)
            {
                Vector2 candidate = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
                if (Mathf.Abs(candidate.x - cursorWorldPoint.x) > half.x + _escapeMargin || Mathf.Abs(candidate.y - cursorWorldPoint.y) > half.y + _escapeMargin)
                {
                    target = new Vector3(candidate.x, candidate.y, target.z);
                    break;
                }
            }
            _buttonRect.position = target;
        }

        private void FinishRunaway()
        {
            _runawayFinished = true;
            _buttonRect.anchoredPosition = _originalAnchoredPosition;
            _buttonText.text = _finishedText;
        }
    }
}
