using CharactersStats;
using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Newtonsoft.Json;
using UnityEngine;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace SaveSystem
{

    public class SavedPlayerStatsProvider
    {
        private const string kStatsFormat = "{0}_Stats";
        private const string PLAYER_TYPE_FORMAT = "AvailablePlayers";
       
        public void SetStats(CharacterType type, OriginStats stats)
        {
            string data = JSON.ToJSON(stats);   
            string key = string.Format(kStatsFormat, type);
            GPrefs.SetString(key, data);
            SetAvailablePlayers(type);
        }

        public OriginStats GetStats(CharacterType type)
        {
            string key = string.Format(kStatsFormat, type);
            string data = GPrefs.GetString(key);
            OriginStats stats = JSON.FromJSON<OriginStats>(data);
            return stats;
        }

        public void SetAvailablePlayers(CharacterType type)
        {

            List<CharacterType> list = GetAvailablePlayers();
            if (list == null)
                list = new List<CharacterType>();
            if(!list.Contains(type))
                list.Add(type);
            string data = JSON.ToJSON(new JsonListWrapper<CharacterType>(list));
            GPrefs.SetString(PLAYER_TYPE_FORMAT,data);
        }

        public List<CharacterType> GetAvailablePlayers()
        {
            string list = GPrefs.GetString(PLAYER_TYPE_FORMAT);
            JsonListWrapper<CharacterType> wrapper;
            wrapper = JSON.FromJSON<JsonListWrapper<CharacterType>>(list);
            if(wrapper == null)
                wrapper = new JsonListWrapper<CharacterType>(new List<CharacterType>());
            List<CharacterType> playerTypes= wrapper.list;
            return playerTypes;
        }
    }

    public interface IDataService
    {
        SavedPlayerStatsProvider PlayerStats { get; }
    }

    public class DataService : IDataService
    {
        public SavedPlayerStatsProvider PlayerStats { get; } = new();
    }

    public class JsonListWrapper<T>
    {
        public List<T> list;
        public JsonListWrapper(List<T> list)
        {
            this.list = list;
        }
    }

    public static class JSON
    {
        public static string ToJSON<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJSON<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        //public static async UniTask<string> ToJSONAsync<T>(T obj)
        //{
        //    return await UniTask.RunOnThreadPool(()
        //        => JsonConvert.SerializeObject(obj));
        //}

        //public static async UniTask<T> FromJSONAsync<T>(string json)
        //{
        //    return await UniTask.RunOnThreadPool<T>(()
        //        => JsonConvert.DeserializeObject<T>(json));
        //}
    }
}


