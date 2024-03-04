using Gameplay;
using Player;
using UnityEngine;
using Zenject;
using Pool;
using Prefab;
using CharactersStats;
using Enemy;
using SlingShotLogic;
public class ContextInstaller : MonoInstaller
{
    #region SerializeFields
    [SerializeField]
    private PlayerPrefabHolder _playerPrefabHolder;
    [SerializeField]
    private EnemyPrefabHolder _enemyPrefabHolder;
    [SerializeField]
    private DefaultScenarioContext _defaultScenarioContext;
    [SerializeField]
    private SlingShotPrefabHolder _slingShotPrefabHolder;
    [SerializeField]
    private DefaultPlayerModelHolder _defaultPlayerModelHolder;
    [SerializeField]
    private SavedCharacterModelHolder _savedCharacterModelHolder;
    [SerializeField]
    private DefaultEnemyModelHolder _defaultEnemyModelHolder;
    #endregion
    public override void InstallBindings()
    {
        BindServices();

        BindScenarios();

        BindPoolers();

        BindFactories();

        BindPrefabHolders();

        BindModelHolders();

        BindControllers();

    }

    #region BindMethods
    public void BindServices()
    {
        Container.Bind<IGameplayService>().To<GameplayService>().AsSingle();
        Container.Bind<IStateData>().To<StateData>().AsSingle();
        Container.Bind<IStatsProvider>().To<StatsProvider>().AsSingle();
    }

    public void BindScenarios()
    {
        Container.Bind<IDefaultScenario>().To<DefaultScenario>().AsTransient();
    }
    public void BindPoolers()
    {
        Container.Bind<ObjectPooler<PlayerType>>().To<PlayerPooler>().AsSingle();
        Container.Bind<ObjectPooler<EnemyType>>().To<EnemyPooler>().AsSingle();
        Container.Bind<ObjectPooler<SlingShotType>>().To<SlingshotPooler>().AsSingle().NonLazy();
    }

    public void BindFactories()
    {
        Container.Bind<IScenarioFactory>().To<ScenarioFactory>().AsSingle();
        Container.Bind<IPlayerFactory>().To<PlayerFactory>().AsSingle();
        Container.Bind<IEnemyFactory>().To<EnemyFactory>().AsSingle();
    }

    public void BindPrefabHolders()
    {
        Container.Bind<PlayerPrefabHolder>().FromInstance(_playerPrefabHolder).AsSingle();
        Container.Bind<EnemyPrefabHolder>().FromInstance(_enemyPrefabHolder).AsSingle();
        Container.Bind<SlingShotPrefabHolder>().FromInstance(_slingShotPrefabHolder).AsSingle();
    }

    public void BindModelHolders()
    {
        Container.Bind<DefaultPlayerModelHolder>().FromInstance(_defaultPlayerModelHolder).AsSingle();
        Container.Bind<DefaultEnemyModelHolder>().FromInstance(_defaultEnemyModelHolder).AsSingle();
        Container.Bind<SavedCharacterModelHolder>().FromInstance(_savedCharacterModelHolder).AsSingle();
    }

    public void BindControllers()
    {
        Container.Bind<IPlayerController>().To<PlayerController>().AsSingle();
        Container.Bind<IEnemyController>().To<EnemyController>().AsSingle();
    }
    #endregion
}
