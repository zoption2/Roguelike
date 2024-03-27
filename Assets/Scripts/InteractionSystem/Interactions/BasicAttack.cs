using CharactersStats;
using Interactions;
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
                //new FireEffect(2),
                //new PoisonEffect(4),
            };
        }

        public override List<IEffect> GetEffects()
        {
            return _effects;
        }

        public override ModifiableStats Interacte(ModifiableStats stats)
        {
            stats.Health.Value -= _damage;
            return stats;
        }
    }
}

