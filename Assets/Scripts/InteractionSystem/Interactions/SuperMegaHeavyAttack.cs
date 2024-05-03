using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class SuperMegaHeavyAttack : InteractionBase
    {

        public SuperMegaHeavyAttack(int damage, int damageMultiplayer) : base(damage)
        {
            _damageMultiplayer = damageMultiplayer;
            _effects = new()
            {
               
            };

            _movable = new StopAndPush();
        }

        public override ReactiveStats InteractWithStats(ReactiveStats stats)
        {
            stats.Health.Value -= _damage * _damageMultiplayer;
            return stats;
        }

        
    }
}
