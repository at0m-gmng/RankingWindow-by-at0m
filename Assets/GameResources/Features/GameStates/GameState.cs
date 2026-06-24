namespace GameResources.Features.GameStates
{
    using Core;
    using PersistentProgress;
    using Signals;
    using UISystem;
    using UISystem.SO;
    using Zenject;

    public sealed class GameState : IState
    {
        public GameState(IGameStateMachine gameStateMachine, IUISystem uiSystem, IPersistentListener persistentListener, SignalBus signalBus)
        {
            _gameStateMachine = gameStateMachine;
            _uiSystem = uiSystem;
            _persistentListener = persistentListener;
            _signalBus = signalBus;
        }
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IUISystem _uiSystem;
        private readonly IPersistentListener _persistentListener;
        private readonly SignalBus _signalBus;

        private RankingWindow _rankingWindow = default;

        public void Enter()
        {
            if (_uiSystem.TryGetWindow(UIWindowID.Ranking, out _rankingWindow))
            {
                _rankingWindow.SetLocalPlayerId(_persistentListener.PlayerId);
                _rankingWindow.SetEntries(_persistentListener.Ranking);
                _signalBus.Subscribe<RankingCloseRequestedSignal>(OnRankingClosed);
                _uiSystem.ShowWindow(UIWindowID.Ranking);
            }
        }

        public void Exit()
        {
            _signalBus.Unsubscribe<RankingCloseRequestedSignal>(OnRankingClosed);
            _uiSystem.HideWindow(UIWindowID.Ranking);
        }

        private void OnRankingClosed() => _gameStateMachine.Enter<MenuState>();
    }
}
