using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using System;
using Prefab;
using Zenject;

namespace Pool
{
    public interface IPooler
    {
        public void Init();
        public IPlayerView GetView<IPlayerView>(Enum _enum,Vector2 position,Quaternion rotation, Transform parent);
    }

    public  class ObjectPooler : IPooler
    {
        private List<Pool> pools;
        public Dictionary<PlayerType, Queue<GameObject>> poolDictionary;
        [Inject]
        private IPrefabByEnumProvider _provider;

        public void Init()
        {
            poolDictionary = new Dictionary<PlayerType, Queue<GameObject>>();
            foreach (Pool pool in pools)
            {
                poolDictionary.Add(pool.tag, new Queue<GameObject>());
            }
        }
        public void CreateObject(PlayerType tag, Vector2 position, Quaternion rotation)
        {
            if (poolDictionary.ContainsKey(tag))
            {
                GameObject prefab = poolDictionary[tag].Dequeue();
                GameObject obj = GameObject.Instantiate(prefab, position, rotation);
                // do some staff with object
            }
        }

        public void Pull(PlayerType tag, Vector2 position, Quaternion rotation)
        {
            if (poolDictionary.ContainsKey(tag))
            {
                GameObject obj = poolDictionary[tag].Dequeue();
                // do some staff with object (set position or etc)
            }
        }
        public void Push(PlayerType tag, GameObject obj)
        {
            obj.SetActive(false);
            if (poolDictionary.ContainsKey(tag))
                poolDictionary[tag].Enqueue(obj);
        }

        public IPlayerView GetView<IPlayerView>(Enum _enum, Vector2 position, Quaternion rotation, Transform parent)
        {
            GameObject prefab = _provider.GetPrefab(PlayerType.Warrior);
            GameObject instance = GameObject.Instantiate(prefab, position, rotation, parent);
            IPlayerView view = instance.GetComponent<IPlayerView>();
            return view;
        }

        [Serializable]
        public class Pool
        {
            public PlayerType tag;
            public GameObject prefab;
            public int size;
        }
    }
}
