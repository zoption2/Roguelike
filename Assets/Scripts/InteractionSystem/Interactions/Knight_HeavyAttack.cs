using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class Knight_HeavyAttack : InteractionBase
    {
        private int _damageMultiplayer;
        public Knight_HeavyAttack(int damage, int damageMultiplayer) : base(damage)
        {
            _damageMultiplayer = damageMultiplayer;
        }

        public override ModifiableStats Interacte(ModifiableStats stats)
        {
            stats.Health.Value -= _damage * _damageMultiplayer;
            return stats;
        }
    }
}


