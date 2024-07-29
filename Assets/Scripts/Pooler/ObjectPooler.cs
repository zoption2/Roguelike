using Prefab;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Pool
{
    public interface IMyPoolable
    {
        GameObject gameObject { get; }
        Transform transform { get; }
        void OnCreate();
        void OnPull();
        void OnRelease();
    }

    public interface IPool<TEnum>
    {
        void Init();
        void Init(Transform parent, string poolName);
        void CleanPool();
        T Pull<T>(TEnum tag, Vector3 position, Quaternion rotation, Transform parent) where T : IMyPoolable;
        void Push(TEnum tag, IMyPoolable obj);
    }

    public abstract class ObjectPooler<TEnum> : IPool<TEnum> where TEnum : Enum
    {
        protected Dictionary<TEnum, Queue<IMyPoolable>> _poolDictionary;
        protected PrefabHolder<TEnum> _prefabHolder;

        protected GameObject GetPrefab(TEnum tag)
        {
            GameObject prefab =  _prefabHolder.GetPrefab(tag);
            return prefab;
        }

        private Transform _parentTransform;
        private static Transform _globalParentTransform;
        private string _poolName;

        public void Init()
        {
            _poolDictionary = new Dictionary<TEnum, Queue<IMyPoolable>>();
        }

        public void Init(Transform globalParent, string poolName)
        {
            _poolDictionary = new Dictionary<TEnum, Queue<IMyPoolable>>();
            _globalParentTransform = globalParent;
            _poolName = poolName;
        }

        public void CleanPool()
        {
            _poolDictionary.Clear();
            _prefabHolder.ReleaseAllAssets();
        }



        private Transform ParentTransform
        {
            get
            {
                if (_parentTransform == null)
                {
                    GameObject parentObject = new GameObject(_poolName);
                    _parentTransform = parentObject.transform;
                    _parentTransform.SetParent(_globalParentTransform);
                    _parentTransform.localPosition = Vector3.zero;
                    _parentTransform.localRotation = Quaternion.identity;
                }
                return _parentTransform;
            }
        }

        public T Pull<T>(TEnum tag, Vector3 position, Quaternion rotation, Transform parent = null) where T : IMyPoolable
        {
            if (_parentTransform == null)
            {
                _parentTransform = ParentTransform;
            }

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
                GameObject prefab =  GetPrefab(tag);

                GameObject spawnedInstance = null;

                if (this is SlingshotPooler)
                {
                    spawnedInstance = ProjectContext.Instance.Container.InstantiatePrefab(prefab, position, rotation, parent);
                }
                else
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

        public void Push(TEnum tag, IMyPoolable obj)
        {
            obj.transform.SetParent(ParentTransform);
            obj.gameObject.SetActive(false);
            if (_poolDictionary.ContainsKey(tag))
            {
                _poolDictionary[tag].Enqueue(obj);
            }
            else
            {
                var queue = new Queue<IMyPoolable>();
                queue.Enqueue(obj);
                _poolDictionary[tag] = queue;
            }
            obj.OnRelease();
        }
    }
}
