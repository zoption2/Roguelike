using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PrefabProvider
{
    public class PrefabProviderByType : MonoBehaviour, IPrefabProviderByType
    {
        public List<GameObject> _prefabs;

        public PrefabProviderByType()
        {
            _prefabs = new List<GameObject>();
        }

        public GameObject GetPrefab<TType>() where TType : MonoBehaviour
        {
            for (int i = 0; i < _prefabs.Count; i++)
            {
                if (_prefabs[i].TryGetComponent(out TType type))
                {
                    return _prefabs[i];
                }
            }
            throw new ArgumentException(
                string.Format("Prefab of type {0} not exists at holder", typeof(TType)));
        }

    }
}
