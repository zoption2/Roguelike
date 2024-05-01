using CharactersStats;
using System;
using System.Collections.Generic;

[Serializable]
public class CharacterModel : CharacterModelBase
{
    public CharacterType Type;
    public List<InteractionType> Abilities;

    private OriginStats _originStats;

    public int Health, Damage, Speed;
    public float LaunchPower, Velocity;

    public CharacterModel(OriginStats originStats, CharacterType type) : base(originStats)
    {
        Type = type;
        Health = _health;
        Damage = _damage;
        Speed = _speed;
        LaunchPower = _launchPower;
        Velocity = _velocity;

        Abilities = new List<InteractionType>
        {
            InteractionType.BasicAttack,
            InteractionType.Knight_HeavyAttack
        };
    }
}

