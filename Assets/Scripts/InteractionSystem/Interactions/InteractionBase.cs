using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public abstract class InteractionBase : IInteraction
    {
        protected int _damage;
        protected IEffect _effect;

        public InteractionBase(
            int damage)
        { 
            _damage = damage;
        }

        public abstract ModifiableStats Interacte(ModifiableStats stats);
        public abstract IEffect GetEffect();
    }
}



