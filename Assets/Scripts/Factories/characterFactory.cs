using CharactersStats;
using Enemy;
using Player;
using Pool;
using Prefab;
using UnityEngine;
using Zenject;

public interface ICharacterController
{
    public bool IsActive { get; set; }
    public event OnEndTurn ON_END_TURN;
    public void Init(CharacterModel model, CharacterView playerView);
}

public abstract class CharacterFactory<TController, TPool>
    where TController : ICharacterController
    where TPool : ObjectPooler<CharacterType>
{
    protected IStatsProvider _statsProvider;
    protected DiContainer _container;
    protected TPool _characterPooler;
    protected CharacterModel _characterModel;
    protected Stats _stats;
    protected CharacterType _type;
    protected IMyPoolable _poolable;


    public CharacterFactory(
        DiContainer container,
        IStatsProvider statsProvider,
        TPool pooler)
    {
        _container = container;
        _statsProvider = statsProvider;
        _characterPooler = pooler;
    }

    protected virtual TController CreateCharacter(Transform point, CharacterType type)
    {
        TController controller = GetNewController();

        _stats = GetStats(type);
        _characterModel = new CharacterModel(_stats, type);

        TPool pooler = GetPooler();
        _poolable = pooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
        CharacterView characterView = _poolable.gameObject.GetComponent<CharacterView>();

        controller.Init(_characterModel, characterView);

        Debug.Log($"Player {type} was created. He spawned on {point.position}");

        return controller;
    }

    protected abstract TPool GetPooler();
    protected abstract Stats GetStats(CharacterType type);

    protected TController GetNewController()
    {
        return _container.Resolve<TController>();
    }
}