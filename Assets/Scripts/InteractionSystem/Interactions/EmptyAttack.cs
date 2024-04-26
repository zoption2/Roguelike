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
            _reloadTime = 0;
            _damageMultiplayer = 0;
            _attackType = TypeOfAttack.MeleeAttack;
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


