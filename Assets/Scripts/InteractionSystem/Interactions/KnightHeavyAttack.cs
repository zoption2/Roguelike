using CharactersStats;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class KnightHeavyAttack : InteractionBase
    {
        private int _damageMultiplayer;
        public KnightHeavyAttack(int damage, int damageMultiplayer) : base(damage)
        {
            _damageMultiplayer = damageMultiplayer;

            _effects = new()
            {
                //new StunEffect(2),
                new FireEffect(2),
            };

            _movable = new StopAndPush();
            _damage = 0;
        }

        public override IMovable GetBump()
        {
            return _movable;
        }

        public override List<IEffect> GetEffects()
        {
            return _effects;
        }

        public override ReactiveStats Interacte(ReactiveStats stats)
        {
            stats.Health.Value -= _damage * _damageMultiplayer;
            return stats;
        }
    }
}


