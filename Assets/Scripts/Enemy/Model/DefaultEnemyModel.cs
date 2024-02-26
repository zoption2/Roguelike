using Player;
using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu]
    public class DefaultEnemyModel : DefaultModel<EnemyType>
    {

    }

    public class DefaultModel<T> : ScriptableObject where T : Enum
    {
        public T Type;
        public int Health, Damage, Speed;
    }
}
