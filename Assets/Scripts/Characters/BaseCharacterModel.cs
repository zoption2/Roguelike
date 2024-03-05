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

    private Enum _type;
    public Enum Type { get { return _type; } }

    protected ReactiveInt _damage;
    protected ReactiveInt _health;
    protected ReactiveInt _speed;
    protected ReactiveFloat _velocity;
    public ReactiveFloat Velocity {
        get { return _velocity; }
        set { _velocity = value; }
    }

    public CharacterModelBase(int id, Enum type, Stats stats)
    {
        _id = id;
        _damage = new ReactiveInt(stats.Damage);
        _health = new ReactiveInt(stats.Health);
        _speed = new ReactiveInt(stats.Speed);
        _velocity = new ReactiveFloat(stats.Velocity);
        _type = type;
    }

    public TEnum GetModelType<TEnum>() where TEnum:Enum { return (TEnum)_type; }
}
