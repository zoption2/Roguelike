using CharactersStats;
using Prefab;
using System;
using System.Collections.Generic;

[Serializable]
public class CharacterModel : CharacterModelBase
{
    public CharacterType Type;

    public int Health, Damage, Speed; //Health, 
    public float LaunchPower;

    public CharacterModel(OriginStats stats, CharacterType type) : base(stats)
    {
        Type = type;
        Health = stats.Health;
        Damage = stats.Damage;
        Speed = stats.Speed;
        LaunchPower = stats.LaunchPower;
    }
}

public class SavedModelCollection
{
    public List<CharacterModel> List = new List<CharacterModel>();
}

