using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Concurrent;
using UnityEditor;

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
        
        public async UniTask<GameObject> GetPrefabAsync(T prefabType)
        {
            foreach (Mapper mapper in _references) 
            {
                if (mapper.Key.Equals(prefabType))
                {
                    AssetReferenceGameObject reference = mapper.PrefabReference;
                    
                    if (reference.IsValid())
                    {
                        if (reference.IsDone)
                        {
                            GameObject prefab = reference.OperationHandle.Convert<GameObject>().Result;
                            return prefab;
                        }
                        else
                        {
                            await reference.OperationHandle;
                            GameObject prefab = reference.OperationHandle.Convert<GameObject>().Result;
                            return prefab;
                        }
                    }
                    else
                    {
                        await reference.LoadAssetAsync();

                        if (reference.OperationHandle.Status == AsyncOperationStatus.Succeeded)
                        {
                            GameObject prefab = reference.OperationHandle.Convert<GameObject>().Result;
                            return prefab;
                        }
                    }
                }
            }
            throw new System.ArgumentException(string.Format("Prefab of type {0} not exists at holder", prefabType));
        }

        public void ReleaseOneAsset(T prefabType)
        {
            foreach (Mapper mapper in _references)
            {
                if (mapper.Key.Equals(prefabType) && mapper.PrefabReference.IsValid())
                {
                    mapper.PrefabReference.ReleaseAsset();
                }
            }
        }

        public void ReleaseAllAssets()
        {
            foreach (Mapper mapper in _references)
            {
                if ( mapper.PrefabReference.IsValid())
                {
                    mapper.PrefabReference.ReleaseAsset();
                }
            }
        }
    }
}
