using Prefab;
using SlingShotLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    public class SlingShotPooler : ObjectPooler<ISlingShot, SlingShotType>
    {
        private SlingShotPrefabHolder _provider;
        public SlingShotPooler(SlingShotPrefabHolder provider)
        {
            _provider = provider;
        }
        public override ISlingShot GetElementAndSpawnIfWasntSpawned<ISlingShot>(SlingShotType _tag, Vector2 position, Quaternion rotation)
        {
            GameObject instance;
            ISlingShot view;
            if (_poolDictionary.ContainsKey(_tag))
            {
                instance = _poolDictionary[_tag].Dequeue();
                view = instance.GetComponent<ISlingShot>();
                return view;
            }
            GameObject prefab = _provider.GetPrefab(_tag);
            instance = GameObject.Instantiate(prefab, position, rotation);
            view = instance.GetComponent<ISlingShot>();
            return view;
        }
    }
}
