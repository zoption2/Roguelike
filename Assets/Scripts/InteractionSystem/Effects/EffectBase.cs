using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public abstract class EffectBase : IEffect
    {
        public int Duration { get; set; }
        protected CharacterModel _stats;
        bool _isPositive;
        bool _isOnStart;

        public EffectBase(CharacterModel stats)
        {
            _stats = stats;
        }

        public abstract CharacterModel UseEffect();
    }
}


