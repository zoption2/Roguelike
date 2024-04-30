using CharactersStats;
using Gameplay;
using Interactions;
using Pool;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class CharacterFactory<TController>
    where TController : ICharacterController
{
    protected IStatsProvider _statsProvider;
    protected DiContainer _container;
    protected CharacterPooler _characterPooler;
    protected CharacterModel _characterModel;
    protected OriginStats _stats;
    protected List<IAbility> _abilities;
    protected CharacterType _type;
    protected IMyPoolable _poolable;
    protected IAbilityFactory _abilityFactory;

    public CharacterFactory(
        DiContainer container,
        IStatsProvider statsProvider,
        CharacterPooler pooler, IAbilityFactory interactionFactory)
    {
        _container = container;
        _statsProvider = statsProvider;
        _characterPooler = pooler;
        _characterPooler.Init();
        _abilityFactory = interactionFactory;
    }

    protected virtual TController CreateCharacter(Transform point, CharacterType type)
    {
        TController controller = GetNewController();

        RawMapper mapper = new RawMapper();
        
        _stats = GetStats(type);
        ReactiveStats reactiveStats = _stats.ToReactive();
        List<AbilityType> abilityTypes = GetAbilitiesTypes(type);
        _abilities = CreateAbilities(abilityTypes, reactiveStats);
        
        _characterModel = new CharacterModel(_stats, type, _abilities);
        mapper.Speed = _stats.Speed;

        _poolable = _characterPooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
        CharacterView characterView = _poolable.gameObject.GetComponent<CharacterView>();

        controller.Init(_characterModel, characterView, _characterPooler);

        mapper.Controller = controller;
        DataTransfer.RawMappers.Add(mapper);

        return controller;
    }

    protected List<IAbility> CreateAbilities(List<AbilityType> types,ReactiveStats stats)
    {
        List<IAbility> abilities = new List<IAbility>();
        foreach (AbilityType type in types)
        {
            abilities.Add(_abilityFactory.CreateAbility(type, stats));
        }
        return abilities;
    }
    protected abstract OriginStats GetStats(CharacterType type);
    protected abstract List<AbilityType> GetAbilitiesTypes(CharacterType type);

    protected TController GetNewController()
    {
        return _container.Resolve<TController>();
    }
}