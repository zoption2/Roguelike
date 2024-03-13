using CharactersStats;
using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Player
{
    [Serializable]
    public class PlayerModel : CharacterModelBase
    {
        public PlayerType Type;
        //public PlayerType Type { get { return _type; } }

        public int Health, Damage, Speed;
        public float LaunchPower;

        public PlayerModel(Stats stats, PlayerType playerType, int id) : base(stats, id)
        {
            Type = playerType;
            Health = stats.Health;
            Damage = stats.Damage;
            Speed = stats.Speed;
            LaunchPower = stats.LaunchPower;
        }
    }   

    public class SavedModelCollection
    {
        public List<PlayerModel> List = new List<PlayerModel>();
    }
}
