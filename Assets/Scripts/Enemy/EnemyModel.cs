using CharactersStats;
using Prefab;
using System;
using UnityEngine;

namespace Enemy
{
    public interface ICharacterModel
    {
        int ID { get; }
    }

    [System.Serializable]
    public class EnemyModel : ICharacterModel
    {
        private int _id;
        public int ID { get { return _id; } }

        private EnemyType _type;
        public EnemyType Type { get { return _type; } }
        private ReactiveInt _damage;
        private ReactiveInt _health;
        private ReactiveInt _speed;
        public EnemyModel(int id, EnemyType type, Stats stats)
        {
            _id = id;
            _type = type;
            _damage = new ReactiveInt(stats.Damage);
            _health = new ReactiveInt(stats.Health);
            _speed = new ReactiveInt(stats.Speed);
        }
    }
}

