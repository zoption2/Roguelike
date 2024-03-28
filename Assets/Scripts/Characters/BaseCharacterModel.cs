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
        _damage = originStats.Damage;
        _health = originStats.Health;
        _speed = originStats.Speed;
        _launchPower = originStats.LaunchPower;
        _velocity = originStats.Velocity;
        _stats = originStats;
    }

    public OriginStats GetStats()
    {
        return _stats;
    }
}
