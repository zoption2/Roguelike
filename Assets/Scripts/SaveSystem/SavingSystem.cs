using CharactersStats;
using JetBrains.Annotations;
using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SaveSystem
{

    public class SavedPlayerStatsProvider
    {
        private const string kStatsFormat = "{0}_Stats";
        private const string PLAYER_TYPE_FORMAT = "AvailablePlayers";
       
        public void SetStats(PlayerType type, Stats stats)
        {
            string data = JsonUtility.ToJson(stats);
            string key = string.Format(kStatsFormat, type);
            GPrefs.SetString(key, data);
            SetAvailablePlayers(type);
        }

        public Stats GetStats(PlayerType type)
        {
            string key = string.Format(kStatsFormat, type);
            string data = GPrefs.GetString(key);
            Stats stats = JsonUtility.FromJson<Stats>(data);
            return stats;
        }

        public void SetAvailablePlayers(PlayerType type)
        {

            List<PlayerType> list = GetAvailablePlayers();
            if (list == null)
                list = new List<PlayerType>();
            if(!list.Contains(type))
                list.Add(type);
            string data = JsonUtility.ToJson(new JsonListWrapper<PlayerType>(list));
            GPrefs.SetString(PLAYER_TYPE_FORMAT,data);
        }

        public List<PlayerType> GetAvailablePlayers()
        {
            string list = GPrefs.GetString(PLAYER_TYPE_FORMAT);
            JsonListWrapper<PlayerType> wrapper;
            wrapper = JsonUtility.FromJson<JsonListWrapper<PlayerType>>(list);
            if(wrapper == null)
                wrapper = new JsonListWrapper<PlayerType>(new List<PlayerType>());
            List<PlayerType> playerTypes= wrapper.list;
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
}


