using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu]
    public class AbilityMapper : ScriptableObject
    {
        public AbilityType Key;
        public InteractionType InteractionType;
        public ProjectileType ProjectileType;
        public int ReloadTime;
        public float LaunchModifier;
        public int RicochetCount;
    }
}
