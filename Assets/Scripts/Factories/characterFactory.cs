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
    public event OnEndTurn OnEndTurn;
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

public interface IPlayerFactory
{
    public IPlayerController CreatePlayer(Transform point, CharacterType type);
}

public class PlayerFactory : CharacterFactory<IPlayerController, PlayerPooler>, IPlayerFactory
{
    private PlayerPooler _pooler;
    public PlayerFactory(DiContainer container, IStatsProvider statsProvider, PlayerPooler pooler) : base(container, statsProvider, pooler)
    {
        _pooler = pooler;
        pooler.Init();
    }

    protected override Stats GetStats(CharacterType type)
    {
        return _statsProvider.GetPlayerStats(type);
    }
    protected override PlayerPooler GetPooler()
    {
        return _pooler;
    }

    public IPlayerController CreatePlayer(Transform point, CharacterType type)
    {
        return base.CreateCharacter(point, type);
    }
}

public interface IEnemyFactory
{
    public IEnemyController CreateEnemy(Transform point, CharacterType type);
}
public class EnemyFactory : CharacterFactory<IEnemyController, EnemyPooler>, IEnemyFactory
{
    private EnemyPooler _pooler;
    public EnemyFactory(DiContainer container, IStatsProvider statsProvider, EnemyPooler pooler) : base(container, statsProvider, pooler)
    {
        _pooler = pooler;
        _characterPooler.Init();
    }

    protected override Stats GetStats(CharacterType type)
    {
        return _statsProvider.GetEnemyStats(type);
    }

    protected override EnemyPooler GetPooler()
    {
        return _pooler;
    }

    public IEnemyController CreateEnemy(Transform point, CharacterType type)
    {
        return base.CreateCharacter(point, type);
    }

    //public IEnemyController CreateCharacter(Transform point, EnemyType type)
    //{
    //    var characterType = (CharacterType)type;
    //    return CreateCharacter(point, characterType);
    //}
}