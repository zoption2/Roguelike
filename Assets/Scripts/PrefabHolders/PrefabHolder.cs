using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prefab
{
    public enum CharacterType
    {
        none = 0,
        Warrior = 1,
        Wizard = 2,
        Archer = 3,
        Barbarian = 100,
        Thrower = 101,
        Summoner = 102,
    }

    public enum PlayerType
    {
        none = 0,
        Warrior = 1,
        Wizard = 2,
        Archer = 3,
    }

    public enum EnemyType
    {
        Barbarian = 100,
        Thrower = 101,
        Summoner = 102,
    }


    public abstract class PrefabHolder<T> : ScriptableObject where T : Enum
    {
        [Serializable]
        public class Mapper 
        {
            public T Key;
            public GameObject Value;
        }

        [SerializeField]
        protected List<Mapper> _prefabs;
        
         public GameObject GetPrefab(T prefabType)
         {
            for (int i = 0;i < _prefabs.Count;i++)
            {
                if (_prefabs[i].Key.Equals(prefabType))
                {
                    return _prefabs[i].Value;
                }
            }
            throw new System.ArgumentException(string.Format("Prefab of type {0} not exists at holder", prefabType));
         }
    }
}
