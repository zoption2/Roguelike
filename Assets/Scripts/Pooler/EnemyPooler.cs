using Player;
using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Pool
{
    public class EnemyPooler : ObjectPooler<EnemyType>
    {
        private EnemyPrefabHolder _provider;
        public EnemyPooler(EnemyPrefabHolder provider)
        {
            _provider = provider;
        }

        protected override GameObject GetPrefab(EnemyType tag)
        {
            return _provider.GetPrefab(tag);
        }
    }
}
