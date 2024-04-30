using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class EmptyAttack : InteractionBase
    {
        public EmptyAttack(int damage) : base(damage)
        {
            _damageMultiplayer = 0;
            _effects = new()
            {

            };
        }

        public override ReactiveStats InteractWithStats(ReactiveStats stats)
        {
            return stats;
        }
    }
} 


