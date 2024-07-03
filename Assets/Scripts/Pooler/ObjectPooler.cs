using Cysharp.Threading.Tasks;
using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Pool
{
    public interface IMyPoolable 
    {
       public GameObject gameObject { get; }
       public Transform transform { get; }
       public void OnCreate();
       public void OnPull();
       public void OnRelease();
    }

    public interface IPool<TEnum>
    {
        public void Init(Transform parent);
        public void CleanPool();
        public  UniTask<T> Pull<T>(TEnum tag, Vector3 position, Quaternion rotation, Transform parent) where T : IMyPoolable;
        public void Push(TEnum tag, IMyPoolable obj);
    }

    public abstract class ObjectPooler<TEnum> : IPool<TEnum> where TEnum : Enum
    {
        protected Dictionary<TEnum, Queue<IMyPoolable>> _poolDictionary;
        protected PrefabHolder<TEnum> _prefabHolder;

        protected async UniTask<GameObject> GetPrefab(TEnum tag)
        {
            GameObject prefab = await _prefabHolder.GetPrefab(tag);
            return prefab;
        }

        protected Transform _parentTransform;

        public void Init()
        {
            _poolDictionary = new Dictionary<TEnum, Queue<IMyPoolable>>();
        }

        public void Init(Transform parent)
        {
            _poolDictionary = new Dictionary<TEnum, Queue<IMyPoolable>>();
            _parentTransform = parent;
        }

        public void CleanPool()
        {
            _poolDictionary.Clear();
        }

        public async UniTask<T> Pull<T>(TEnum tag, Vector3 position, Quaternion rotation, Transform parent) where T : IMyPoolable
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
                GameObject prefab = await GetPrefab(tag);

                GameObject spawnedInstance = null;

                if (this is SlingshotPooler)
                {
                    spawnedInstance = ProjectContext.Instance.Container.InstantiatePrefab(prefab, position, rotation, parent);
                } else
                {
                    spawnedInstance = GameObject.Instantiate(prefab, position, rotation);
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
            obj.transform.SetParent(_parentTransform);
            if (_poolDictionary.ContainsKey(tag))
            {
                obj.gameObject.SetActive(false);
                _poolDictionary[tag].Enqueue(obj);
                obj.OnRelease();
            }
            else
            {
                _poolDictionary.Add(tag, new Queue<IMyPoolable>());
                _poolDictionary[tag].Enqueue(obj);
                obj.gameObject.SetActive(false);
                obj.OnRelease();
            }
        }

    }
}
