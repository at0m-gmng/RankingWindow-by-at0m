namespace GameResources.Features.Installers
{
    using PersistentProgress;
    using SaveLoadSystem;
    using Signals;
    using UISystem;
    using UISystem.SO;
    using UnityEngine;
    using Zenject;

    public sealed class ProjectInstaller : MonoInstaller
    {
        [Header("Configs")]
        [SerializeField] private UIConfig _uiConfig = default;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            DeclareSignals();
            InstallServices();
            InstallUI();
        }

        private void DeclareSignals()
        {
            Container.DeclareSignal<MenuStartRequestedSignal>();
            Container.DeclareSignal<RankingCloseRequestedSignal>();
        }

        private void InstallServices()
        {
            Container.BindInterfacesTo<SaveLoadService>().AsSingle();
            Container.BindInterfacesTo<PersistentProgressService>().AsSingle();
        }

        private void InstallUI() => Container.BindInterfacesTo<UISystem>().AsSingle().WithArguments(_uiConfig);
    }
}
