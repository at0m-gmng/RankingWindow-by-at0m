using GameResources.Features.PersistentProgress;

namespace GameResources.Features.GameStates
{
    using Core;
    using UISystem;

    public sealed class BootstrapState : IState
    {
        public BootstrapState(IGameStateMachine gameStateMachine, IUISystem uiSystem, IPersistentProgressService persistentProgress)
        {
            _gameStateMachine = gameStateMachine;
            _uiSystem = uiSystem;
            _persistentProgress = persistentProgress;
        }
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IUISystem _uiSystem;
        private readonly IPersistentProgressService _persistentProgress;

        public async void Enter()
        {
            await _uiSystem.Initialize();
            if (!_persistentProgress.TryLoadData())
            {
                _persistentProgress.InitNewProgress();
            }
            _gameStateMachine.Enter<MenuState>();
        }

        public void Exit()
        {
        }
    }
}