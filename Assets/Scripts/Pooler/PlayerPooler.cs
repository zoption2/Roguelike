using Player;
using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Pool {

    public class PlayerPooler : ObjectPooler<CharacterType>
    {
        private PlayerPrefabHolder _provider;
        public PlayerPooler(PlayerPrefabHolder provider)
        {
            _provider = provider;
        }

        protected override GameObject GetPrefab(CharacterType tag)
        {
            return _provider.GetPrefab(tag);
        }
    }
}
