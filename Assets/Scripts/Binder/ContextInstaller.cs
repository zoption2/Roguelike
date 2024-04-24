using Gameplay;
using Player;
using UnityEngine;
using Zenject;
using Pool;
using Prefab;
using CharactersStats;
using Enemy;
using UI;
using Interactions;
using SaveSystem;
using BehaviourTree;
public class ContextInstaller : MonoInstaller
{
    #region SerializeFields
    [SerializeField]
    private CharacterPrefabHolder _characterPrefabHolder;

    [SerializeField]
    private CharacterUIPrefabHolder _characterUIPrefabHolder;

    [SerializeField]
    private CharacterPanelPrefabHolder characterPanelPrefabHolder;

    [SerializeField]
    private DefaultScenarioContext _defaultScenarioContext;

    [SerializeField]
    private SlingShotPrefabHolder _slingShotPrefabHolder;

    [SerializeField]
    private DefaultPlayerModelHolder _defaultPlayerModelHolder;

    [SerializeField]
    private DefaultEnemyModelHolder _defaultEnemyModelHolder;

    [SerializeField]
    private BuffPrefabHolder _buffPrefabHolder;
    [SerializeField]
    private NavigationPrefabHolder _navPrefabHolder;
    //
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

        BindInteractions();

        BindBehaviourTrees();

    }

    #region BindMethods
    public void BindServices()
    {
        Container.Bind<IGameplayService>().To<GameplayService>().AsSingle();
        Container.Bind<IStateFactory>().To<StateData>().AsSingle();
        Container.Bind<IStatsProvider>().To<StatsProvider>().AsSingle();
        Container.Bind<ICharacterSelector>().To<CharacterSelector>().AsSingle();
        Container.Bind<IDataService>().To<DataService>().AsSingle(); 
    }

    public void BindInteractions()
    {
        Container.Bind<IInteractionDealer>().To<InteractionDealer>().AsTransient();
        Container.Bind<IInteractionFactory>().To<InteractionFactory>().AsSingle();

        Container.Bind<IEffectProcessor>().To<EffectProcessor>().AsTransient();
        Container.Bind<IInteractionProcessor>().To<InteractionProcessor>().AsTransient();
        Container.Bind<IInteractionCalculator>().To<InteractionCalculator>().AsTransient();
    }

    public void BindScenarios()
    {
        Container.Bind<IDefaultScenario>().To<DefaultScenario>().AsTransient();
    }
    public void BindPoolers()
    {
        Container.Bind<CharacterPooler>().To<CharacterPooler>().AsSingle();
        Container.Bind<SlingshotPooler>().To<SlingshotPooler>().AsSingle();
        Container.Bind<CharacterPanelPooler>().To<CharacterPanelPooler>().AsSingle();
        Container.Bind<BuffPooler>().To<BuffPooler>().AsSingle();
        Container.Bind<CharacterUIPooler>().To<CharacterUIPooler>().AsSingle();
    }

    public void BindFactories()
    {
        Container.Bind<IScenarioFactory>().To<ScenarioFactory>().AsSingle();
        Container.Bind<IPlayerFactory>().To<PlayerFactory>().AsSingle();
        Container.Bind<IEnemyFactory>().To<EnemyFactory>().AsSingle();
        Container.Bind<ICharacterPanelFactory>().To<CharacterPanelFactory>().AsSingle();
        Container.Bind<INavigationFactory>().To<NavigationFactory>().AsSingle();

    }

    public void BindPrefabHolders()
    {
        Container.Bind<CharacterPrefabHolder>().FromInstance(_characterPrefabHolder).AsSingle();
        Container.Bind<CharacterUIPrefabHolder>().FromInstance(_characterUIPrefabHolder).AsTransient();
        Container.Bind<SlingShotPrefabHolder>().FromInstance(_slingShotPrefabHolder).AsSingle();
        Container.Bind<CharacterPanelPrefabHolder>().FromInstance(characterPanelPrefabHolder).AsSingle();
        Container.Bind<BuffPrefabHolder>().FromInstance(_buffPrefabHolder).AsSingle();
        Container.Bind<NavigationPrefabHolder>().FromInstance(_navPrefabHolder).AsSingle();
    }

    public void BindModelHolders()
    {
        Container.Bind<DefaultPlayerModelHolder>().FromInstance(_defaultPlayerModelHolder).AsSingle();
        Container.Bind<DefaultEnemyModelHolder>().FromInstance(_defaultEnemyModelHolder).AsSingle();
        Container.Bind<ICharacterPanelModel>().To<CharacterPanelModel>().AsTransient();
    }

    public void BindControllers()
    {
        Container.Bind<IPlayerController>().To<PlayerController>().AsTransient();
        Container.Bind<IEnemyController>().To<EnemyController>().AsTransient();
        Container.Bind<ICharacterPanelController>().To<CharacterPanelController>().AsTransient();
    }

    public void BindBehaviourTrees()
    {
        Container.Bind<IDefaultBehaviourTree>().To<DefaultBehaviourTree>().AsTransient();
    }
    #endregion
}
