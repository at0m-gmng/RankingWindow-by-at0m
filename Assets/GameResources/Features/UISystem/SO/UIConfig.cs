namespace GameResources.Features.UISystem.SO
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [CreateAssetMenu(menuName = "Configs/UIConfig", fileName = "UIConfig")]
    public sealed class UIConfig : ScriptableObject
    {
        [field: SerializeField] public GameObject WindowsParent { get; private set; } = default;
        [field: SerializeField] public List<UIData> Windows { get; private set; } = new List<UIData>();
    }

    [Serializable]
    public sealed class UIData
    {
        public AssetReference Reference;
        public UIWindowID ID;
    }

    public enum UIWindowID
    {
        Menu = 10,
        Ranking = 50
    }
}