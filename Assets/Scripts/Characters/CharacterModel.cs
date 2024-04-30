using CharactersStats;
using Interactions;
using Prefab;
using System;
using System.Collections.Generic;

[Serializable]
public class CharacterModel : CharacterModelBase
{
    public CharacterType Type;

    public int Health, Damage, Speed;
    public float LaunchPower, Velocity;
    public List<IAbility> Abilities;

    public CharacterModel(OriginStats originStats, CharacterType type, List<IAbility> abilities) : base(originStats)
    {
        Type = type;
        Abilities = abilities;
        Health = _health;
        Damage = _damage;
        Speed = _speed;
        LaunchPower = _launchPower;
        Velocity = _velocity;
    }

}

