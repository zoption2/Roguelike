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
        Monster1 = 100,
        Monster2 = 101,
        Monster3 = 102,
    }

    public enum MonsterType
    {
        Monster1 = 100,
        Monster2 = 101,
        Monster3 = 102,
    }

    public class DODOD
    {
        void dod(MonsterType monster)
        {
            CharacterType type = (CharacterType)monster;
        }
    }

    public enum PlayerType 
    {
        Warrior = 1 ,
        Wizard = 2,
        Archer = 3,
    }

    public enum EnemyType 
    {
        Barbarian,
        Thrower,
        Summoner,
    }

    public enum SlingShotType
    {
        Melee,
        Distance,
        Magic,
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
