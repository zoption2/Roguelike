using CharactersStats;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions 
{
    public class BasicAttack : InteractionBase
    {
        public BasicAttack(int damage) : base(damage)
        {
            _damageMultiplayer = 1;
            _effects = new()
            {
                new FireEffect(3),
            };

        }

        public override ReactiveStats InteractWithStats(ReactiveStats stats)
        {
            stats.Health.Value -= _damage;
            return stats;
        }
    }
}

