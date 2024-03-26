using CharactersStats;
using Interactions;
using UnityEngine;

namespace Interactions 
{
    public class BasicAttack : InteractionBase
    {
        public BasicAttack(int damage) : base(damage)
        {
            _effect = new Fire_Effect(3);
            //_effect = new MoreDamage_Effect(damage);
        }

        public override IEffect GetEffect()
        {
            return _effect;
        }

        public override ModifiableStats Interacte(ModifiableStats stats)
        {
            stats.Health.Value -= _damage;
            return stats;
        }
    }
}

