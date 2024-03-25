using CharactersStats;
using Interactions;
using UnityEngine;

namespace Interactions 
{
    public class BasicAttack : InteractionBase
    {
        public BasicAttack(int damage) : base(damage)
        {
        }

        public override ModifiableStats Interacte(ModifiableStats stats)
        {
            stats.Health.Value -= _damage;
            return stats;
        }
    }
}

