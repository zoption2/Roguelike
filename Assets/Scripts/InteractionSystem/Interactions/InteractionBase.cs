using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public abstract class InteractionBase : IInteraction
    {
        protected int _damage;
        protected int _damageMultiplayer;
        protected List<IEffect> _effects;
        protected IMovable _movable;

        public InteractionBase(int damage)
        {
            _damage = damage;
            _damageMultiplayer = 1;
            _movable = new Bounce();
        }
        public int GetDamage()
        {
            return _damage;
        }

        public void SetStats(ReactiveStats stats)
        {
            _damage = stats.Damage.Value;
        }
        public ReactiveStats Interact(ReactiveStats stats)
        {
            return InteractWithStats(stats);
        }
        public abstract ReactiveStats InteractWithStats(ReactiveStats stats);
        public  List<IEffect> GetEffects()
        {
            return _effects;
        }
        public IMovable GetBump()
        {
            return _movable;
        }
    }
}



