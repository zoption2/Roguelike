using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using System;
using Prefab;
using Zenject;

namespace Pool
{
    public abstract  class ObjectPooler<T, TEnum> where TEnum : Enum
    {
        protected List<Pool<TEnum>> _pools;
        protected Dictionary<TEnum, Queue<GameObject>> _poolDictionary;
        // construct
        public void Init()
        {
            _poolDictionary = new Dictionary<TEnum, Queue<GameObject>>();
            //foreach (Pool<TEnum> pool in _pools)
            //{
            //    _poolDictionary.Add(pool.Tag, new Queue<GameObject>());
            //}
        }
        public void CreateObject(TEnum tag, Vector2 position, Quaternion rotation)
        {
            if (_poolDictionary.ContainsKey(tag))
            {
                GameObject prefab = _poolDictionary[tag].Dequeue();
                GameObject obj = GameObject.Instantiate(prefab, position, rotation);
                // do some staff with object
            }
        }

        public void Pull(TEnum tag, Vector2 position, Quaternion rotation)
        {
            if (_poolDictionary.ContainsKey(tag))
            {
                GameObject obj = _poolDictionary[tag].Dequeue();
                // do some staff with object (set position or etc)
            }
        }
        public void Push(TEnum tag, GameObject obj)
        {
            obj.SetActive(false);
            if (_poolDictionary.ContainsKey(tag))
                _poolDictionary[tag].Enqueue(obj);
        }

        public abstract T GetElementAndSpawnIfWasntSpawned<T>(TEnum _tag, Vector2 position, Quaternion rotation, Transform parent);
        //{
        //    GameObject prefab = _provider.GetPrefab(_tag);
        //    // Check
        //    GameObject instance = GameObject.Instantiate(prefab, position, rotation, parent);
        //    IPlayerView view = instance.GetComponent<IPlayerView>();
        //    return view;
        //}

        [Serializable]
        public class Pool<TEnum> where TEnum : Enum
        {
            public TEnum Tag;
            public GameObject Prefab;
        }
    }
}
