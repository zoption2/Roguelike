using CharactersStats;
using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    [Serializable]
    public class SavedModel
    {
        public int ID;
        public PlayerType Type;
        public int Health, Damage, Speed;

        public SavedModel(Stats stats,int id,PlayerType playerType)
        {
            ID = id;
            Health = stats.Health;
            Damage = stats.Damage;
            Speed = stats.Speed;
            Type = playerType;
        }
    }   

    public class SavedModelCollection
    {
        public List<SavedModel> List = new List<SavedModel>();
    }
}
