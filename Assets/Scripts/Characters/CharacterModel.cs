using CharactersStats;
using Interactions;
using System;
using System.Collections.Generic;

[Serializable]
public class CharacterModel : CharacterModelBase
{
    public CharacterType Type;
    public List<InteractionType> Abilities;
    private ReactiveList<IEffect> _allEffects;

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

        _allEffects = new ReactiveList<IEffect>();

        _allEffects.Value = new List<IEffect>();

        Abilities = new List<InteractionType>
        {
            InteractionType.BasicAttack,
            InteractionType.Knight_HeavyAttack
        };
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

