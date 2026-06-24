namespace GameResources.Features.GameStates
{
    using Core;
    using Signals;
    using UISystem;
    using UISystem.SO;
    using Zenject;

    public sealed class MenuState : IState
    {
        public MenuState(IGameStateMachine gameStateMachine, IUISystem uiSystem, SignalBus signalBus)
        {
            _gameStateMachine = gameStateMachine;
            _uiSystem = uiSystem;
            _signalBus = signalBus;
        }
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IUISystem _uiSystem;
        private readonly SignalBus _signalBus;

        public void Enter()
        {
            if (_uiSystem.TryGetWindow(UIWindowID.Menu, out MenuWindow _))
            {
                _signalBus.Subscribe<MenuStartRequestedSignal>(OnStartRequested);
                _uiSystem.ShowWindow(UIWindowID.Menu);
            }
        }

        public void Exit()
        {
            _signalBus.Unsubscribe<MenuStartRequestedSignal>(OnStartRequested);
            _uiSystem.HideWindow(UIWindowID.Menu);
        }

        private void OnStartRequested() => _gameStateMachine.Enter<GameState>();
    }
}
