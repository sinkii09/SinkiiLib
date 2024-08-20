using SinkiiLib.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SinkiiLib.Systems
{
    public class SaveLoadSystem : Singleton<SaveLoadSystem>
    {
        [SerializeField] public GameData gameData;

        IDataService dataService;

        public override void Awake()
        {
            base.Awake();
            dataService = new FileDataService(new JsonSerializer());
        }
        public void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null)
            {
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                }
                entity.Bind(data);
            }
        }
        public void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            foreach (var entity in entities)
            {
                var data = datas.FirstOrDefault(d => d.Id == entity.Id);
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }
        public void NewGame(Action OnLoadData)
        {
            gameData = new GameData
            {
                Name = "New Game",
                PlayerData = new PlayerData
                {
                    position = new Vector3(0, 0, 0)
                }
            };
            OnLoadData?.Invoke();
        }
        public void SaveGame() => dataService.Save(gameData);

        public void LoadGame(string gameName, Action OnLoadData)
        {
            gameData = dataService.Load(gameName);
            OnLoadData?.Invoke();
        }
        public void ReloadGame(Action OnLoadData) => LoadGame(gameData.Name, OnLoadData);

        public void DeleteGame(string gameName) => dataService.Delete(gameName);
    }
}
