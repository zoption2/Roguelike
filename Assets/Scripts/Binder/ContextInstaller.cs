using Gameplay;
using Player;
using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Pool;
using Prefab;
using Enemy;

public class ContextInstaller : MonoInstaller
{
    [SerializeField] private PlayerPrefabHolder _playerPrefabHolder;
    [SerializeField] private SlingShotPrefabHolder _slingShotPrefabHolder;
    [SerializeField] private DefaultPlayerModelHolder _defaultPlayerModelHolder;
    [SerializeField] private SavedCharacterModelHolder _savedCharacterModelHolder;
    [SerializeField] private EnemyPrefabHolder _enemyPrefabHolder;
    [SerializeField] private DefaultEnemyModelHolder _enemyModelHolder;

    public override void InstallBindings()
    {
        Container.Bind<IStatsProvider>().To<StatsProvider>().AsSingle();
        Container.Bind<IScenarioFactory>().To<ScenarioFactory>().AsSingle();
        Container.Bind<IPlayerFactory>().To<PlayerFactory>().AsSingle();
        Container.Bind<DefaultPlayerModelHolder>().FromInstance(_defaultPlayerModelHolder).AsSingle();
        Container.Bind<SavedCharacterModelHolder>().FromInstance(_savedCharacterModelHolder).AsSingle();
        Container.Bind<IPlayerController>().To<PlayerController>().AsSingle();
        Container.Bind<ObjectPooler<PlayerType>>().To<PlayerPooler>().AsSingle();
        Container.Bind<ObjectPooler<SlingShotType>>().To<SlingshotPooler>().AsSingle().NonLazy();
        Container.Bind<PlayerPrefabHolder>().FromInstance(_playerPrefabHolder).AsSingle();
        Container.Bind<SlingShotPrefabHolder>().FromInstance(_slingShotPrefabHolder).AsSingle();
        Container.Bind<IGameplayService>().To<GameplayService>().AsSingle();

        Container.Bind<IEnemyController>().To<EnemyController>().AsTransient();
        Container.Bind<ObjectPooler<EnemyType>>().To<EnemyPooler>().AsSingle();
        Container.Bind<EnemyPrefabHolder>().FromInstance(_enemyPrefabHolder).AsSingle();
        Container.Bind<DefaultEnemyModelHolder>().FromInstance(_enemyModelHolder).AsSingle();
        Container.Bind<IEnemyFactory>().To<EnemyFactory>().AsSingle();
    }
}
