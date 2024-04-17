using CharactersStats;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions 
{
    public class BasicAttack : InteractionBase
    {
        public BasicAttack(int damage) : base(damage)
        {
            _effects = new()
            {
                new FireEffect(3),
            };

        }

        public override IMovable GetBump()
        {
            return _movable;
        }

        public override List<IEffect> GetEffects()
        {
            return _effects;
        }

        public override ReactiveStats Interacte(ReactiveStats stats)
        {
            stats.Health.Value -= _damage;
            return stats;
        }
    }
}

