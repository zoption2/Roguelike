using CharactersStats;
using Gameplay;
using Pool;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Abilities;
using Cysharp.Threading.Tasks;

public abstract class CharacterFactory<TController>
    where TController : ICharacterController
{
    protected IStatsProvider _statsProvider;
    protected DiContainer _container;
    protected CharacterPooler _characterPooler;
    protected CharacterUIPooler _characterUIPooler;
    protected CharacterModel _characterModel;
    protected OriginStats _stats;
    protected List<IAbility> _abilities;
    protected CharacterType _type;
    protected IMyPoolable _poolable;
    protected IAbilityFactory _abilityFactory;

    public CharacterFactory(
        DiContainer container,
        IStatsProvider statsProvider,
        IPoolManager poolManager,
         IAbilityFactory abilityFactory)
    {
        _container = container;
        _statsProvider = statsProvider;
        _characterPooler = poolManager.GetCharacterPooler();
        _characterUIPooler = poolManager.GetCharacterUIPooler();
        _abilityFactory = abilityFactory;
    }

    protected TController CreateCharacter(Vector3 position, Transform parent, CharacterType type)
    {
        TController controller = GetNewController();

        RawMapper mapper = new RawMapper();
        
        _stats = GetStats(type);
        ReactiveStats reactiveStats = _stats.ToReactive();
        List<AbilityType> abilityTypes = GetAbilitiesTypes(type);
        _abilities = CreateAbilities(abilityTypes, reactiveStats);
        
        _characterModel = new CharacterModel(_stats, type, _abilities);
        mapper.Speed = _stats.Speed;

        _poolable =  _characterPooler.Pull<IMyPoolable>(type, position, Quaternion.identity, parent);
        CharacterView characterView = _poolable.gameObject.GetComponent<CharacterView>();

        _poolable =  _characterUIPooler.Pull<IMyPoolable>(UIType.CharacterUI, position, Quaternion.Euler(90, 0, 0), parent);
        CharacterUIView characterUIView = _poolable.gameObject.GetComponent<CharacterUIView>();

        controller.Init(_characterModel, characterView, characterUIView);

        mapper.Controller = controller;
        DataTransfer.RawMappers.Add(mapper);

        return controller;
    }

    protected List<IAbility> CreateAbilities(List<AbilityType> types,ReactiveStats stats)
    {
        List<IAbility> abilities = new List<IAbility>();

        if (!types.Contains(AbilityType.BasicAttackAbility))
        {
            abilities.Add(_abilityFactory.CreateAbility(AbilityType.BasicAttackAbility, stats));
        }
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