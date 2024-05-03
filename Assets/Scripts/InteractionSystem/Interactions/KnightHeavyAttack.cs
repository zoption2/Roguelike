using CharactersStats;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class KnightHeavyAttack : InteractionBase
    {
        public KnightHeavyAttack(int damage, int damageMultiplayer) : base(damage)
        {
            _damageMultiplayer = damageMultiplayer;
            _effects = new()
            {
                new StunEffect(2),
                //new FireEffect(2),
            };

            _movable = new StopAndPush();
            //_damage = 0;
        }

        public override ReactiveStats InteractWithStats(ReactiveStats stats)
        {
            stats.Health.Value -= _damage * _damageMultiplayer;
            return stats;
        }
    }
}


