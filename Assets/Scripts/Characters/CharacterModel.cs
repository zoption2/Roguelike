using CharactersStats;
using Interactions;
using Prefab;
using System;
using System.Collections.Generic;

[Serializable]
public class CharacterModel : CharacterModelBase
{
    public CharacterType Type;
    private ReactiveList<IEffect> _allEffects;

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

        _allEffects = new ReactiveList<IEffect>();

        _allEffects.Value = new List<IEffect>();
    }

    public void SetHealth(int value)
    {
        _reactiveStats.Health.Value = value;
    }

    public ReactiveList<IEffect> GetAllEffects()
    {
        return _allEffects; 
    }

}

