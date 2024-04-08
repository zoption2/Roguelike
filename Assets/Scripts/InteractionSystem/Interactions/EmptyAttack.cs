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
            _effects = new()
            {

            };
        }
        public override List<IEffect> GetEffects()
        {
            return _effects;
        }

        public override ReactiveStats Interacte(ReactiveStats stats)
        {
            return stats;
        }
    }
} 


