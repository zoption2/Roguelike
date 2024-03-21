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

        public override Stats Interacte(Stats stats)
        {
            stats.Health -= _damage;
            return stats;
        }
    }
}

