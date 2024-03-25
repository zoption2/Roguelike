using CharactersStats;
using Prefab;
using System;
using System.Collections.Generic;

[Serializable]
public class CharacterModel : CharacterModelBase
{
    public CharacterType Type;

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

public class SavedModelCollection
{
    public List<CharacterModel> List = new List<CharacterModel>();
}

