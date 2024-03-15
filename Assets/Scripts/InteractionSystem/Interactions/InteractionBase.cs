using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public abstract class InteractionBase : IInteraction
    {
        protected int _damage;

        public InteractionBase(
            int damage)
        { 
            _damage = damage;
        }

        public abstract Stats Interacte(Stats stats);
    }
}



