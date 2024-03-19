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
            Debug.LogWarning($"HP before attack: {stats.Health}");
            stats.Health -= _damage;
            Debug.LogWarning($"HP after attack: {stats.Health}");
            return stats;
        }
    }
}

