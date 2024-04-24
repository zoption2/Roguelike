using CharactersStats;
using System;

[Serializable]
public class CharacterModel : CharacterModelBase
{
    public CharacterType Type;

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
    }
}

