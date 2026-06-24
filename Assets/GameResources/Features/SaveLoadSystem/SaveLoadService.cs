using UnityEngine;

namespace GameResources.Features.SaveLoadSystem
{
    public sealed class SaveLoadService : ISaveLoadService
    {
        private const string SAVE_CATALOG = "Saves/";

        private string Path { get; set; }

        public void SaveData<T>(string data)
        {
            Path = SAVE_CATALOG + typeof(T).Name;
            Save(data);
        }
        public void SaveData<T>(T data)
        {
            Path = SAVE_CATALOG + typeof(T).Name;
            Save(JsonUtility.ToJson(data));
        }

        public bool TryLoadData<T>(out T data)
        {
            Path = SAVE_CATALOG + typeof(T).Name;
            data = JsonUtility.FromJson<T>(Load());
            return data != null;
        }
        
        private void Save(string value)
        {
            PlayerPrefs.SetString(Path, value);
            PlayerPrefs.Save();
        }
        private string Load() => PlayerPrefs.GetString(Path, string.Empty);
    }

    public interface ISaveLoadService
    {
        public bool TryLoadData<T>(out T data);
        public void SaveData<T>(string data);
        public void SaveData<T>(T data);
    }
}