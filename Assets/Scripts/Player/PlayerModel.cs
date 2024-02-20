
using Prefab;
using System;
using UnityEngine;

namespace Player
{
    public interface ICharacterModel
    {
        int ID { get;}
    }

    [System.Serializable]
    public class PlayerModel: ICharacterModel
    {
        private int _id;
        public int ID { get { return _id; } }

        private PlayerType _type;
        public PlayerType Type { get { return _type; } }
        private ReactiveInt Damage, Health, Speed;
        public PlayerModel(int id,PlayerType type,Stats stats)
        {
            _id = id;
            _type = type;
            Damage = new ReactiveInt(stats.Damage);
            Health = new ReactiveInt(stats.Health);
            Speed = new ReactiveInt(stats.Speed);
        }
    }
}

