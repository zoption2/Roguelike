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
        Archer
    }

    public enum SlingShotType
    {
        //Maybe here will be different types of slingshots
    }
    public interface IPrefabByEnumProvider
    {
        GameObject GetPrefab<TEnum>(TEnum prefabType) where TEnum : System.Enum;
    }
    public interface IPrefabByTypeProvider
    {
        GameObject GetPrefab<TType>(TType prefabType) where TType : System.Type;
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

        //public GameObject GetPrefab<TEnum>(TEnum prefabType) where TEnum : Enum
        //{
        //    for (int i = 0;i < _prefabs.Count;i++)
        //    {
        //        if (_prefabs[i].PlayerType.Equals(prefabType))
        //        {
        //            return _prefabs[i].Value;
        //        }
        //    }
        //    throw new System.ArgumentException(string.Format("Prefab of type {0} not exists at holder", prefabType));
        //}
        
         public GameObject GetPrefab<T>(T prefabType)
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
