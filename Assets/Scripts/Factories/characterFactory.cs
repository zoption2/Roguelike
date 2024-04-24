using CharactersStats;
using Gameplay;
using Pool;
using UnityEngine;
using Zenject;

public abstract class CharacterFactory<TController>
    where TController : ICharacterController
{
    protected IStatsProvider _statsProvider;
    protected DiContainer _container;
    protected CharacterPooler _characterPooler;
    protected CharacterUIPooler _characterUIPooler;
    protected CharacterModel _characterModel;
    protected OriginStats _stats;
    protected CharacterType _type;
    protected IMyPoolable _poolable;

    public CharacterFactory(
        DiContainer container,
        IStatsProvider statsProvider,
        CharacterPooler pooler,
        CharacterUIPooler characterUIPooler)
    {
        _container = container;
        _statsProvider = statsProvider;
        _characterPooler = pooler;
        _characterUIPooler = characterUIPooler;
        _characterPooler.Init();
        _characterUIPooler.Init();
    }

    protected virtual TController CreateCharacter(Transform point, CharacterType type)
    {
        TController controller = GetNewController();

        RawMapper mapper = new RawMapper();

        _stats = GetStats(type);
        _characterModel = new CharacterModel(_stats, type);
        mapper.Speed = _stats.Speed;

        _poolable = _characterPooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
        CharacterView characterView = _poolable.gameObject.GetComponent<CharacterView>();

        _poolable = _characterUIPooler.Pull<IMyPoolable>(UIType.CharacterUI, point.position, point.rotation, point.parent);
        CharacterUIView characterUIView = _poolable.gameObject.GetComponent<CharacterUIView>();

        controller.Init(_characterModel, characterView, _characterPooler, characterUIView);

        mapper.Controller = controller;
        DataTransfer.RawMappers.Add(mapper);

        return controller;
    }

    protected abstract OriginStats GetStats(CharacterType type);

    protected TController GetNewController()
    {
        return _container.Resolve<TController>();
    }
}