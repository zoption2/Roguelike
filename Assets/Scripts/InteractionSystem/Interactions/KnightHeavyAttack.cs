using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class KnightHeavyAttack : InteractionBase
    {
        private int _damageMultiplayer;
        public KnightHeavyAttack(int damage, int damageMultiplayer) : base(damage)
        {
            _damageMultiplayer = damageMultiplayer;

            _effects = new()
            {
                
            };
        }

        public override List<IEffect> GetEffects()
        {
            return _effects;
        }

        public override ReactiveStats Interacte(ReactiveStats stats)
        {
            stats.Health.Value -= _damage * _damageMultiplayer;
            return stats;
        }
    }
}


