using Enemy;
using Gameplay;
using Pool;
using Prefab;
using CharactersStats;
using UnityEngine;
using Zenject;

public interface IEnemyFactory
{
    public IEnemyController CreateEnemy(Transform point, EnemyType type, ICharacterScenarioContext characters, int id = 0);
}
public class EnemyFactory : IEnemyFactory
{
    private IStatsProvider _statsProvider;
    private DiContainer _container;
    private ObjectPooler<EnemyType> _enemyPooler;

    [Inject]
    public void Construct(
        DiContainer container,
        IStatsProvider statsProvider,
        ObjectPooler<EnemyType> enemyPooler)
    {
        _container = container;
        _statsProvider = statsProvider;
        _enemyPooler = enemyPooler;
        _enemyPooler.Init();
    }

    public IEnemyController CreateEnemy(Transform point, EnemyType type, ICharacterScenarioContext characters, int id = 0)
    {
        IEnemyController controller;
        IEnemyView enemyView;
        Transform poolableTransform;
        Rigidbody2D enemyViewRigidbody;
        EnemyModel enemyModel;
        Stats stats;

        controller = GetNewController();

        stats = _statsProvider.GetEnemyStats(type, id);

        enemyModel = new EnemyModel(id, type, stats);

        IMyPoolable _poolable = _enemyPooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
        enemyView = _poolable.gameObject.GetComponent<MyEnemyView>();

        GameObject _poolableObj = _poolable.gameObject.transform.GetChild(0).gameObject;
        poolableTransform = _poolableObj.transform;

        enemyViewRigidbody = _poolable.gameObject.GetComponentInChildren<Rigidbody2D>();

        enemyView.Initialize((EnemyController)controller);

        controller.Init(poolableTransform, enemyViewRigidbody, enemyModel);

        Debug.Log($"Enemy of type {type} was created. They have {stats.Health} hp and spawned on {point.position}");

        return controller;
    }

    private IEnemyController GetNewController()
    {
        return _container.Resolve<IEnemyController>();
    }
}


