using Player;
using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Pool {
    public class PlayerPooler : ObjectPooler<IPlayerView,PlayerType>
    {
        private PlayerPrefabHolder _provider;
        public PlayerPooler(PlayerPrefabHolder provider)
        {
            _provider = provider;
        }
        public override IPlayerView GetElementAndSpawnIfWasntSpawned<IPlayerView>(PlayerType _tag, Vector2 position, Quaternion rotation, Transform parent)
        {
            GameObject instance;
            IPlayerView view;
            if (_poolDictionary.ContainsKey(_tag))
            {
                instance = _poolDictionary[_tag].Dequeue();
                view = instance.GetComponent<IPlayerView>();
                return view;
            }
            GameObject prefab = _provider.GetPrefab(_tag);
            instance = GameObject.Instantiate(prefab, position, rotation, parent);
            view = instance.GetComponent<IPlayerView>();
            return view;
        }
    }
}
