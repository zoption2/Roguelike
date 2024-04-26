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
        protected int _turnsLeftToReload;
        protected int _damageMultiplayer;
        protected float _launchMultiplier;
        protected List<IEffect> _effects;
        protected IMovable _movable;
        protected TypeOfAttack _attackType;

        public InteractionBase(
            int damage)
        {
            _damage = damage;
            _readyForUse = true;
            _damageMultiplayer = 1;
            _turnsLeftToReload = 0;
            _launchMultiplier = 1;
            _movable = new Bounce();
        }
        public bool CouldUseAbility()
        {
            return _readyForUse;
        }

        public float GetLaunchMultiplier()
        {
            return _launchMultiplier;
        }
        public int GetDamage()
        {
            return _damage;
        }

        public TypeOfAttack GetAttackType()
        {
            return _attackType;
        }

        public void SetStats(ReactiveStats stats)
        {
            _damage = stats.Damage.Value;
        }
        public void TickReload()
        {
            _turnsLeftToReload--;
            Debug.Log( _turnsLeftToReload + " turns left till reload of "+ this );
            if (_turnsLeftToReload == 0)
            {
                _readyForUse = true;
                Debug.Log(this + " has reloaded!");
            }
                
        }
        protected void SetForReload()
        {
            _turnsLeftToReload = _reloadTime;
            if(_turnsLeftToReload > 0)
            {
                _readyForUse = false;
                Debug.Log(this + " is now reloading. It will be reloading for " + _turnsLeftToReload + " turns");
            }
        }
        public ReactiveStats Interact(ReactiveStats stats)
        {
            SetForReload();
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



