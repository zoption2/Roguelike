using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu]
    public class AbilityHolder : ScriptableObject
    {
        [SerializeField]
        protected List<AbilityMapper> _abilities;

        public AbilityMapper GetAbilityData(AbilityType abilityType)
        {
            for (int i = 0; i < _abilities.Count; i++)
            {
                if (_abilities[i].Key.Equals(abilityType))
                {
                    return _abilities[i];
                }
            }
            throw new System.ArgumentException(string.Format("Ability of type {0} not exists", abilityType));
        }
    }
}
