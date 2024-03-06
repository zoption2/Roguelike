using CharactersStats;
using Prefab;
using System;
using UnityEngine;

public interface ICharacterModel
{
    int ID { get; }
}
public abstract class CharacterModelBase : ICharacterModel
{
    private int _id;
    public int ID { get { return _id; } }

    private ReactiveFloat _velocity;
    public ReactiveFloat Velocity { get { return _velocity; } }

    protected ReactiveInt _damage;
    protected ReactiveInt _health;
    protected ReactiveInt _speed;

    public CharacterModelBase(int id, Stats stats)
    {
        _id = id;
        _damage = new ReactiveInt(stats.Damage);
        _health = new ReactiveInt(stats.Health);
        _speed = new ReactiveInt(stats.Speed);
        _velocity = new ReactiveFloat(0f);
    }
}
