namespace GameResources.Features.Installers
{
    using GameStates;
    using GameStates.Core;
    using Zenject;

    public sealed class GameInstaller : MonoInstaller
    {
        public override void InstallBindings() => InstallStateMachine();

        private void InstallStateMachine()
        {
            Container.BindInterfacesTo<GameStateMachine>().AsSingle();
            Container.BindInterfacesTo<BootstrapState>().AsSingle();
            Container.BindInterfacesTo<MenuState>().AsSingle();
            Container.BindInterfacesTo<GameState>().AsSingle();
        }
    }
}