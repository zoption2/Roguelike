using CharactersStats;
using Prefab;
using System;
using UnityEngine;

public interface ICharacterModel
{
    
}
public abstract class CharacterModelBase : ICharacterModel
{

    private ReactiveFloat _velocity;
    private ReactiveInt _health;

    protected ReactiveInt _damage;
    //protected ReactiveInt _health;
    protected ReactiveInt _speed;
    protected ReactiveFloat _launchPower;
    private OriginStats _stats;

    public ReactiveFloat Velocity { get { return _velocity; } }
    public ReactiveInt ReactiveHealth { get { return _health; } }

    public CharacterModelBase(OriginStats stats)
    {
        _damage = new ReactiveInt(stats.Damage);
        _health = new ReactiveInt(stats.Health);
        _speed = new ReactiveInt(stats.Speed);
        _launchPower = new ReactiveFloat(stats.LaunchPower);
        _velocity = new ReactiveFloat(0f);
        _stats = stats;
    }

    public OriginStats GetStats()
    {
        return _stats;
    }
}
