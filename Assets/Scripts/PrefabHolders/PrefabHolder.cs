using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Concurrent;

namespace Prefab
{
    public abstract class PrefabHolder<T> : ScriptableObject where T : Enum
    {
        [Serializable]
        public class Mapper 
        {
            public T Key;
            public AssetReferenceGameObject PrefabReference;
        }

        [SerializeField]
        protected List<Mapper> _references;
        protected ConcurrentDictionary<string, AsyncOperationHandle<GameObject>> _cache = new();
        
        public async UniTask<GameObject> GetPrefab(T prefabType)
        {
            foreach (Mapper mapper in _references) 
            {
                if (mapper.Key.Equals(prefabType))
                {
                    AssetReferenceGameObject reference = mapper.PrefabReference;
                    string key = reference.RuntimeKey.ToString();

                    if (_cache.TryGetValue(key,out AsyncOperationHandle<GameObject> handle))
                    {
                        GameObject prefab = handle.Result;

                        return prefab;
                    }
                    else
                    {                        
                        AsyncOperationHandle<GameObject> handler = Addressables.LoadAssetAsync<GameObject>(reference.RuntimeKey);
                        await handler;

                        if (handler.Status == AsyncOperationStatus.Succeeded)
                        {
                            _cache.TryAdd(key, handler);

                            GameObject prefab = handler.Result;
                            return prefab;
                        }
                    }
                }
            }
            throw new System.ArgumentException(string.Format("Prefab of type {0} not exists at holder", prefabType));
        }
    }
}
