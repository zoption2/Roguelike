using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public abstract class InteractionBase : IInteraction
    {
        protected int _damage;
        protected List<IEffect> _effects;

        public InteractionBase(
            int damage)
        { 
            _damage = damage;
        }

        public abstract ReactiveStats Interacte(ReactiveStats stats);
        public abstract List<IEffect> GetEffects();
    }
}



