namespace GameResources.Features.UISystem
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public sealed class AddressableSpriteCache : IDisposable
    {
        private readonly Dictionary<string, Sprite> _cache = new();
        private readonly Dictionary<string, AsyncOperationHandle<Sprite>> _handles = new();

        public void Load(string address, Action<Sprite> onLoaded)
        {
            if (_cache.TryGetValue(address, out Sprite cached))
            {
                onLoaded(cached);
            }
            else if (_handles.TryGetValue(address, out AsyncOperationHandle<Sprite> pending))
            {
                pending.Completed += handle => InvokeIfSucceeded(handle, onLoaded);
            }
            else
            {
                AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(address);
                _handles[address] = handle;
                handle.Completed += completed =>
                {
                    if (completed.Status == AsyncOperationStatus.Succeeded && completed.Result != null)
                    {
                        _cache[address] = completed.Result;
                    }
                    InvokeIfSucceeded(completed, onLoaded);
                };
            }
        }

        public void Dispose()
        {
            foreach (AsyncOperationHandle<Sprite> handle in _handles.Values)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            _handles.Clear();
            _cache.Clear();
        }

        private void InvokeIfSucceeded(AsyncOperationHandle<Sprite> handle, Action<Sprite> onLoaded)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            {
                onLoaded(handle.Result);
            }
        }
    }
}
