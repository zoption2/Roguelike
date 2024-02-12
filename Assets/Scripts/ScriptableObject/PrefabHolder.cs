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
    public interface IPrefabByEnumProvider
    {
        GameObject GetPrefab<TEnum>(TEnum prefabType) where TEnum : System.Enum;
    }
    [CreateAssetMenu]
    public class PrefabHolder : ScriptableObject, IPrefabByEnumProvider
    {
        [Serializable]
        public class Mapper
        {
            public PlayerType Player_Type;
            public GameObject Value;
        }

        public List<Mapper> prefabs;

        public GameObject GetPrefab<TEnum>(TEnum prefabType) where TEnum : Enum
        {
            for (int i = 0;i < prefabs.Count;i++)
            {
                if (prefabs[i].Player_Type.Equals(prefabType))
                {
                    return prefabs[i].Value;
                }
            }
            throw new System.ArgumentException(string.Format("Prefab of type {0} not exists at holder", prefabType));
        }
    }
}
