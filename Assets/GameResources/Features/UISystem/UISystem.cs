namespace GameResources.Features.UISystem
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using SO;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Zenject;

    public sealed class UISystem : IUISystem
    {
        public UISystem(UIConfig uiConfig, DiContainer container)
        {
            _uiConfig = uiConfig;
            _container = container;
        }
        private readonly UIConfig _uiConfig;
        private readonly DiContainer _container;

        private GameObject _windowsParent;
        private readonly Dictionary<UIWindowID, BaseWindow> _windows = new();
        private readonly Dictionary<UIWindowID, AssetReference> _windowReferences = new();
        
        public async UniTask Initialize()
        {
            _windowsParent = Object.Instantiate(_uiConfig.WindowsParent);
            
            foreach (UIData window in _uiConfig.Windows)
            {
                await LoadAndCreateWindow(window.ID, window.Reference);
            }
        }
        public void ShowWindow(UIWindowID id)
        {
            if (_windows.TryGetValue(id, out BaseWindow window))
            {
                window.Show();
            }
        }
        
        public void HideWindow(UIWindowID id)
        {
            if (_windows.TryGetValue(id, out BaseWindow window))
            {
                window.Hide();
            }
        }

        public bool TryGetWindow<T>(UIWindowID id, out T window) where T : BaseWindow
        {
            window = null;
            if (_windows.TryGetValue(id, out BaseWindow cachedWindow))
            {
                window = cachedWindow as T;
            }
            return window != null;
        }

        public UniTask ReleaseWindow(UIWindowID id)
        {
            if (_windows.TryGetValue(id, out BaseWindow window))
            {
                if (window != null)
                {
                    Object.Destroy(window.gameObject);
                }
                _windows.Remove(id);
            }
            if (_windowReferences.TryGetValue(id, out AssetReference reference))
            {
                reference.ReleaseAsset();
                _windowReferences.Remove(id);
            }
            return UniTask.CompletedTask;
        }

        public UniTask ReleaseAllWindows()
        {
            UIWindowID[] windowIDs = _windows.Keys.ToArray();
            foreach (UIWindowID id in windowIDs)
            {
                ReleaseWindow(id);
            }
            return UniTask.CompletedTask;
        }
        
        private async UniTask LoadAndCreateWindow(UIWindowID id, AssetReference reference)
        {
            UniTask<GameObject> handle = reference.LoadAssetAsync<GameObject>().ToUniTask();
            (bool IsCanceled, GameObject Result) isResult = await handle.SuppressCancellationThrow();
            
            if (!isResult.IsCanceled && isResult.Result != null)
            {
                BaseWindow window = _container.InstantiatePrefabForComponent<BaseWindow>(isResult.Result, _windowsParent.transform);
                window.gameObject.SetActive(false);
                _windows[id] = window;
                _windowReferences[id] = reference;
            }
        }
    }

    public interface IUISystem
    {
        public UniTask Initialize();
        public void ShowWindow(UIWindowID id);
        public void HideWindow(UIWindowID id);
        public bool TryGetWindow<T>(UIWindowID id, out T window) where T : BaseWindow;
        public UniTask ReleaseWindow(UIWindowID id);
        public UniTask ReleaseAllWindows();
    }
}