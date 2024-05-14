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
        public void Init();
        public void CleanPool();
        public T Pull<T>(TEnum tag, Vector3 position, Quaternion rotation, Transform parent) where T : IMyPoolable;
        public void Push(TEnum tag, IMyPoolable obj);
    }

    public abstract class ObjectPooler<TEnum> : IPool<TEnum>
    {
        protected Dictionary<TEnum, Queue<IMyPoolable>> _poolDictionary;

        protected abstract GameObject GetPrefab(TEnum tag);

        public void Init()
        {
            _poolDictionary = new Dictionary<TEnum, Queue<IMyPoolable>>();
        }

        public void CleanPool()
        {
            _poolDictionary.Clear();
        }
        public T Pull<T>(TEnum tag, Vector3 position, Quaternion rotation, Transform parent) where T : IMyPoolable
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                _poolDictionary.Add(tag, new Queue<IMyPoolable>());
            }
            var selectedQueue = _poolDictionary[tag];
            if (selectedQueue.Count > 0)
            {
                Debug.LogWarning("this is " + this);
                Debug.LogWarning("tag :" + tag + "  " + selectedQueue.Count);
                foreach (var A in  selectedQueue)
                {
                    Debug.LogWarning(A);
                }
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
                } else
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
