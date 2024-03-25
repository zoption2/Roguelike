using CharactersStats;
using Prefab;
using System;
using UnityEngine;

public interface ICharacterModel
{
    
}
public abstract class CharacterModelBase : ICharacterModel
{

    protected float _velocity;
    protected int _damage;
    protected int _health;
    protected int _speed;
    protected float _launchPower;
    private OriginStats _stats;

    public CharacterModelBase(OriginStats originStats)
    {
        _damage = originStats.Damage.Value;
        _health = originStats.Health.Value;
        _speed = originStats.Speed.Value;
        _launchPower = originStats.LaunchPower.Value;
        _velocity = originStats.Velocity.Value;
        _stats = originStats;
    }

    public OriginStats GetStats()
    {
        return _stats;
    }
}
