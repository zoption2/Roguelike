using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public abstract class InteractionBase : IInteraction
    {
        protected int _damage;
        protected bool _readyForUse;
        protected int _reloadTime;
        protected List<IEffect> _effects;
        protected IMovable _movable;

        public InteractionBase(
            int damage)
        { 
            _damage = damage;
            _movable = new Bounce();
        }
        public bool CouldUseAbility()
        {
            return _readyForUse;
        }
        public int GetDamage()
        {
            return _damage;
        }
        public abstract ReactiveStats Interacte(ReactiveStats stats);
        public abstract List<IEffect> GetEffects();
        public abstract IMovable GetBump();
    }
}



