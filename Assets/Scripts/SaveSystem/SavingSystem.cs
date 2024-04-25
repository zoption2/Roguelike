using CharactersStats;
using Prefab;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Interactions;

namespace SaveSystem
{
    public class SavedPlayerDataProvider
    {
        private const string kStatsFormat = "{0}_Stats";
        private const string kAbilitiesFormat = "{0}_Abilities";
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
        public void SetAbilities(List<InteractionType> abilityTypes,CharacterType type)
        {
            string key = string.Format(kAbilitiesFormat, type);
            string data = JSON.ToJSON(new JsonListWrapper<InteractionType>(abilityTypes));
            GPrefs.SetString(key, data);
        }

        public List<InteractionType> GetAbilities(CharacterType type)
        {
            string key = string.Format(kAbilitiesFormat, type);
            string data = GPrefs.GetString(key);
            JsonListWrapper<InteractionType> wrapper;
            wrapper = JSON.FromJSON<JsonListWrapper<InteractionType>>(data);
            List<InteractionType> avilitiesTypes = wrapper.list;
            return avilitiesTypes;
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
        SavedPlayerDataProvider PlayerData { get; }
    }

    public class DataService : IDataService
    {
        public SavedPlayerDataProvider PlayerData { get; } = new();
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


