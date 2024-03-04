using System;
using CharactersStats;
using Enemy;
using Gameplay;
using Player;
using Pool;
using Prefab;
using UnityEngine;
using Zenject;

public interface ICharacterController
{
    public bool IsActive { get; set; }
    public event OnSwitchState OnSwitch;
    public void Init(Transform poolableTransform, Rigidbody2D rigidbody, CharacterModel model, CharacterView playerView);

}

public interface ICharacterFactory<TController, TEnum>
    where TController : ICharacterController
    where TEnum : Enum
{
    TController CreateCharacter(Transform point, TEnum type, ICharacterScenarioContext characters, int id = 0);
}

public abstract class CharacterFactory<TController, TEnum> : ICharacterFactory<TController, TEnum>
    where TController : ICharacterController
    where TEnum : Enum
{
    protected IStatsProvider _statsProvider;
    private DiContainer _container;
    protected abstract ObjectPooler<TEnum> _pooler { get; }

    public CharacterFactory(
        DiContainer container,
        IStatsProvider statsProvider)
    {
        _container = container;
        _statsProvider = statsProvider;
    }

    public TController CreateCharacter(Transform point, TEnum type, ICharacterScenarioContext characters, int id = 0)
    {
        CharacterView characterView;
        TController controller;
        Transform poolableTransform;
        Rigidbody2D characterRigidbody;
        CharacterModel characterModel;
        Stats stats;

        controller = GetNewController();

        stats = GetStats(type, id);

        characterModel = new CharacterModel(id, type, stats);

        IMyPoolable poolable = _pooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
        characterView = poolable.gameObject.GetComponent<CharacterView>();

        GameObject poolableObject = poolable.gameObject.transform.GetChild(0).gameObject;
        poolableTransform = poolableObject.transform;

        characterRigidbody = poolable.gameObject.GetComponentInChildren<Rigidbody2D>();

        controller.Init(poolableTransform, characterRigidbody, characterModel, characterView);

        //Debug.Log($"Character of type {type} with id {id} was created. They have {stats.Health} hp and spawned on {point.position}");

        return controller;
    }

    protected abstract Stats GetStats(TEnum playerType, int id);

    private TController GetNewController()
    {
        return _container.Resolve<TController>();
    }
}