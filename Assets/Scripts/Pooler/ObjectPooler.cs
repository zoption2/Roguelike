using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using System;
using Prefab;
using Zenject;
using UnityEditor;

namespace Pool
{
    public interface IMyPoolable 
    {
       GameObject gameObject { get; }
       void OnCreate();
       void OnPull();
       void OnRelease();
    }

    public interface IPool<TEnum> where TEnum : Enum 
    {
        public void Init();
        public T Pull<T>(TEnum tag, Vector2 position, Quaternion rotation, Transform parent) where T : IMyPoolable;
        public void Push(TEnum tag, IMyPoolable obj);
    }

    public abstract class ObjectPooler<TEnum> : IPool<TEnum> where TEnum : Enum
    {
        protected Dictionary<TEnum, Queue<IMyPoolable>> _poolDictionary;

        protected abstract GameObject GetPrefab(TEnum tag);

        public void Init()
        {
            _poolDictionary = new Dictionary<TEnum, Queue<IMyPoolable>>();
        }
        

        public T Pull<T>(TEnum tag, Vector2 position, Quaternion rotation, Transform parent) where T : IMyPoolable
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                _poolDictionary.Add(tag, new Queue<IMyPoolable>());
            }
            var selectedQueue = _poolDictionary[tag];
            if (selectedQueue.Count > 0)
            {
                IMyPoolable resultObject = _poolDictionary[tag].Dequeue();
                resultObject.gameObject.transform.position = position;
                resultObject.gameObject.transform.rotation = rotation;
                resultObject.gameObject.transform.SetParent(parent);
                resultObject.gameObject.SetActive(true);
                resultObject.OnPull();
                return (T)resultObject;
            }
            else
            {
                var prefab = GetPrefab(tag);

                GameObject spawnedInstance = null;

                if (this is SlingshotPooler)
                {
                    spawnedInstance = ProjectContext.Instance.Container.InstantiatePrefab(prefab, position, rotation, parent);
                }
                else if (this is PlayerPooler)
                {
                    spawnedInstance = GameObject.Instantiate(prefab, position, rotation, parent);
                }
                IMyPoolable result = spawnedInstance.gameObject.GetComponent<IMyPoolable>();
                result.gameObject.transform.position = position;
                result.gameObject.transform.rotation = rotation;
                result.gameObject.transform.SetParent(parent);
                result.gameObject.SetActive(true);
                result.OnCreate();
                return (T)result;
            }
        }
        public void Push(TEnum tag,IMyPoolable obj)
        {
            if (_poolDictionary.ContainsKey(tag))
            {
                obj.gameObject.SetActive(false);
                _poolDictionary[tag].Enqueue(obj);
                obj.OnRelease();
            }
        }

    }
}
