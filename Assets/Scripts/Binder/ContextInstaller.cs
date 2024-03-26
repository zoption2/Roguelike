using Gameplay;
using Player;
using UnityEngine;
using Zenject;
using Pool;
using Prefab;
using CharactersStats;
using Enemy;
using SlingShotLogic;
using UI;
using Interactions;
public class ContextInstaller : MonoInstaller
{
    #region SerializeFields
    [SerializeField]
    private CharacterPrefabHolder _characterPrefabHolder;

    [SerializeField]
    private CharacterPanelPrefabHolder characterPanelPrefabHolder;

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

        Container.Bind<IInteractionDealer>().To<InteractionDealer>().AsTransient();
        Container.Bind<IInteractionFactory>().To<InteractionFactory>().AsSingle();

        Container.Bind<IEffectProcessor>().To<EffectProcessor>().AsTransient();
        Container.Bind<IInteractionProcessor>().To<InteractionProcessor>().AsTransient();
        Container.Bind<IInteractionFinalizer>().To<InteractionFinalizer>().AsTransient();
    }

    #region BindMethods
    public void BindServices()
    {
        Container.Bind<IGameplayService>().To<GameplayService>().AsSingle();
        Container.Bind<IStateData>().To<StateData>().AsSingle();
        Container.Bind<IStatsProvider>().To<StatsProvider>().AsSingle();
        Container.Bind<ICharacterSelector>().To<CharacterSelector>().AsSingle();
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
    }

    public void BindFactories()
    {
        Container.Bind<IScenarioFactory>().To<ScenarioFactory>().AsSingle();
        Container.Bind<IPlayerFactory>().To<PlayerFactory>().AsSingle();
        Container.Bind<IEnemyFactory>().To<EnemyFactory>().AsSingle();
        Container.Bind<ICharacterPanelFactory>().To<CharacterPanelFactory>().AsSingle();

    }

    public void BindPrefabHolders()
    {
        Container.Bind<CharacterPrefabHolder>().FromInstance(_characterPrefabHolder).AsSingle();
        Container.Bind<SlingShotPrefabHolder>().FromInstance(_slingShotPrefabHolder).AsSingle();
        Container.Bind<CharacterPanelPrefabHolder>().FromInstance(characterPanelPrefabHolder).AsSingle();
    }

    public void BindModelHolders()
    {
        Container.Bind<DefaultPlayerModelHolder>().FromInstance(_defaultPlayerModelHolder).AsSingle();
        Container.Bind<DefaultEnemyModelHolder>().FromInstance(_defaultEnemyModelHolder).AsSingle();
        Container.Bind<ISavedCharacterModelHolder>().To<SavedCharacterModelHolder>().AsSingle();
        Container.Bind<ICharacterPanelModel>().To<CharacterPanelModel>().AsTransient();
    }

    public void BindControllers()
    {
        Container.Bind<IPlayerController>().To<PlayerController>().AsTransient();
        Container.Bind<IEnemyController>().To<EnemyController>().AsTransient();
        Container.Bind<ICharacterPanelController>().To<CharacterPanelController>().AsTransient();
    }
    #endregion
}
