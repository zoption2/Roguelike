using System.Collections;
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
        void Init(Transform parent);
        void CleanPool();
        T Pull<T>(TEnum tag, Vector3 position, Quaternion rotation, Transform parent) where T : IMyPoolable;
        void Push(TEnum tag, IMyPoolable obj);
    }

    public abstract class ObjectPooler<TEnum> : IPool<TEnum>
    {
        protected Dictionary<TEnum, Queue<IMyPoolable>> _poolDictionary;

        protected abstract GameObject GetPrefab(TEnum tag);

        private Transform _parentTransform;
        private Transform _globalParentTransform;

        public void Init()
        {
            _poolDictionary = new Dictionary<TEnum, Queue<IMyPoolable>>();
        }

        public void Init(Transform globalParent)
        {
            _poolDictionary = new Dictionary<TEnum, Queue<IMyPoolable>>();
            _globalParentTransform = globalParent;
        }

        public void CleanPool()
        {
            _poolDictionary.Clear();
        }

        private Transform ParentTransform
        {
            get
            {
                if (_parentTransform == null)
                {
                    string poolName = typeof(TEnum).Name;
                    if (poolName.Length > 4)
                    {
                        poolName = poolName.Substring(0, poolName.Length - 4);
                    }
                    GameObject parentObject = new GameObject(poolName + " Pool");
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
                GameObject parentObject = new GameObject(typeof(TEnum).Name + " Pool");
                _parentTransform = parentObject.transform;
                _parentTransform.SetParent(_globalParentTransform);
                _parentTransform.localPosition = Vector3.zero;
                _parentTransform.localRotation = Quaternion.identity;
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
                var prefab = GetPrefab(tag);
                GameObject spawnedInstance;

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
