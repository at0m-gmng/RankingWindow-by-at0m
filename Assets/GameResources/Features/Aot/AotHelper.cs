namespace GameResources.Features.Aot
{
    using GameStates;
    using GameStates.Core;
    using PersistentProgress;
    using SaveLoadSystem;
    using UISystem.SO;
    using UnityEngine;
    using UnityEngine.Scripting;

    public sealed class AotHelper : MonoBehaviour
    {
        [Header("Configs")]
        [SerializeField] private UIConfig _uiConfig = default;

        [Preserve]
        private void PreserveGenericConstructors()
        {
            UISystem.UISystem uiSystem = new UISystem.UISystem(_uiConfig, null);
            SaveLoadService saveLoad = new SaveLoadService();
            PersistentProgressService persistentProgress = new PersistentProgressService(saveLoad);
            GameStateMachine stateMachine = new GameStateMachine();
            _ = new BootstrapState(stateMachine, uiSystem, persistentProgress);
            _ = new MenuState(stateMachine, uiSystem, null);
            _ = new GameState(stateMachine, uiSystem, persistentProgress, null);
        }
    }
}
