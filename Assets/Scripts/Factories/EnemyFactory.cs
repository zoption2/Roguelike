//using CharactersStats;
//using Enemy;
//using Pool;
//using Prefab;
//using Zenject;

//public interface IEnemyFactory : ICharacterFactory<IEnemyController, EnemyType> { }
//public class EnemyFactory : CharacterFactory<IEnemyController, EnemyType>, IEnemyFactory
//{
//    private EnemyPooler _concretePooler;
//    protected override ObjectPooler<EnemyType> _pooler { get => _concretePooler; }

//    public EnemyFactory(
//        DiContainer container,
//        IStatsProvider statsProvider,
//        EnemyPooler enemyPooler)
//        : base(container, statsProvider)
//    {
//        _concretePooler = enemyPooler;
//        _concretePooler.Init();
//    }
//    protected override Stats GetStats(EnemyType enemyType, int id)
//    {
//        return _statsProvider.GetCharacterStats(enemyType, id);
//    }
//}

//using CharactersStats;
//using Player;
//using Pool;
//using Prefab;
//using Zenject;

//public interface IPlayerFactory : ICharacterFactory<IPlayerController, PlayerType> { }
//public class PlayerFactory : CharacterFactory<IPlayerController, PlayerType>, IPlayerFactory
//{
//    private PlayerPooler _concretePooler;
//    protected override ObjectPooler<PlayerType> _pooler { get => _concretePooler; }

//    public PlayerFactory(
//        DiContainer container,
//        IStatsProvider statsProvider,
//        PlayerPooler playerPooler)
//        : base(container, statsProvider)
//    {
//        _concretePooler = playerPooler;
//        _concretePooler.Init();
//    }

//    protected override Stats GetStats(PlayerType playerType, int id)
//    {
//        return _statsProvider.GetCharacterStats(playerType, id);
//    }
//}

using Gameplay;
using Player;
using CharactersStats;
using Pool;
using Prefab;
using UnityEngine;
using Zenject;
using Enemy;

public interface IEnemyFactory
{
    public IEnemyController CreateEnemy(Transform point, EnemyType type, int id = 0);
}
public class EnemyFactory : IEnemyFactory
{
    private IStatsProvider _statsProvider;
    private DiContainer _container;
    private EnemyPooler _enemyPooler;

    [Inject]
    public void Construct(
        DiContainer container,
        IStatsProvider statsProvider,
        EnemyPooler enemyPooler)
    {
        _container = container;
        _statsProvider = statsProvider;
        _enemyPooler = enemyPooler;
        _enemyPooler.Init();
    }

    public IEnemyController CreateEnemy(Transform point, EnemyType type, int id = 0)
    {
        CharacterView enemyView;
        IEnemyController controller;
        EnemyModel enemyModel;
        Stats stats;

        controller = GetNewController();

        stats = _statsProvider.GetEnemyStats(type, id);

        enemyModel = new EnemyModel(stats, type, id);

        IMyPoolable _poolable = _enemyPooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
        enemyView = _poolable.gameObject.GetComponent<CharacterView>();

        controller.Init(enemyModel, enemyView);

        Debug.Log($"Player {type} with id {id} was created. They have {stats.Health} hp and spawned on {point.position}");

        return controller;
    }

    private IEnemyController GetNewController()
    {
        return _container.Resolve<IEnemyController>();
    }
}