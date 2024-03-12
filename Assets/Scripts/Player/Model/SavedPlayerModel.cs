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
    public class SavedPlayerModel : CharacterModelBase
    {
        public PlayerType Type;
        //public PlayerType Type { get { return _type; } }

        public int Health, Damage, Speed;
        public float LaunchPower;

        public SavedPlayerModel(Stats stats, PlayerType playerType, int id) : base(stats, id)
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
        public List<SavedPlayerModel> List = new List<SavedPlayerModel>();
    }
}
