using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu]
    public class DefaultPlayerModel : DefaultModel<CharacterType>
    {
     
    }

    public class DefaultModel<T> : ScriptableObject where T : Enum
    {
        public T Type;
        public int Health, Damage, Speed;
        public float LaunchPower;
        public List<AbilityType> Abilities;
    }
}
