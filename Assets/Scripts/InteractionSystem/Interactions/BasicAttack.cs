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

        public override CharacterModel Interacte(CharacterModel stats)
        {
            stats.ReactiveHealth.Value -= _damage;
            return stats;
        }
    }
}

