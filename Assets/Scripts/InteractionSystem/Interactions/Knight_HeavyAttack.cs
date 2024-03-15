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

        public override Stats Interacte(Stats stats)
        {
            stats.Health -= _damage * _damageMultiplayer;
            return stats;
        }
    }
}


