using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prefab
{
    public enum PlayerType
    {
        Warrior,
        Wizard,
        Archer,
    }

    public enum SlingShotType
    {
        Melee,
        Distance,
        Magic,
    }
    
    [CreateAssetMenu]
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
        
         public GameObject GetPrefab<P>(P prefabType) //P is the same as T
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
