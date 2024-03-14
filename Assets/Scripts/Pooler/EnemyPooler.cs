using Player;
using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Pool
{
    public class EnemyPooler : ObjectPooler<CharacterType>
    {
        private EnemyPrefabHolder _provider;
        public EnemyPooler(EnemyPrefabHolder provider)
        {
            _provider = provider;
        }

        protected override GameObject GetPrefab(CharacterType tag)
        {
            return _provider.GetPrefab(tag);
        }
    }
}
