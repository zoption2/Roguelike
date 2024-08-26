using CharactersStats;
using Pool;

namespace Gameplay
{
    public interface IStateFactory
    {
        public void Init(IScenario scenarioInstance, RoomContext context);
        public IState CreateState(TypeOfState type);
        public IConditionState CreateConditionState(TypeOfConditionState type, ICharacterController controller);
    }
    public class StateData : IStateFactory
    {
        private IBuffFactory _triggerFactory;
        private IPlayerFactory _playerFactory;
        private IEnemyFactory _enemyFactory;
        private IRoomObjectsFactory _roomObjectsFactory;
        private IStatsProvider _statsProvider;
        private IScenario _scenarioInstance;
        private RoomContext _roomContext;
        private INavigationFactory _navigationFactory;
        private ProjectilePooler _projectilePooler;
        private ILevelManager _levelManager;
        private ILevelContext _levelContext;
        private IRoomBuilder _roomBuilder;
        private ICameraManager _cameraManager;

        public StateData(
            IBuffFactory triggerFactory,
            IPlayerFactory playerFactory,
            IEnemyFactory enemyFactory,
            IStatsProvider statsProvider,
            INavigationFactory navigationFactory,
            IPoolManager poolManager,
            IRoomObjectsFactory roomObjectsFactory,
            ILevelManager levelManager,
            ILevelContext levelContext,
            IRoomBuilder roomBuilder,
            ICameraManager cameraManager)
        {
            _triggerFactory = triggerFactory;
            _enemyFactory = enemyFactory;
            _playerFactory = playerFactory;
            _statsProvider = statsProvider;
            _navigationFactory = navigationFactory;
            _roomObjectsFactory = roomObjectsFactory;
            _levelManager = levelManager;
            _levelContext = levelContext;
            _roomBuilder = roomBuilder;
            _cameraManager = cameraManager;
            _projectilePooler = poolManager.UseProjectilePooler();
        }

        public void Init(IScenario scenarioInstance, RoomContext context)
        {
            _roomContext = context;
            _scenarioInstance = scenarioInstance;
        }

        public IState CreateState(TypeOfState type)
        {
            IState state = null;
            switch (type)
            {
                case TypeOfState.Init:
                    state = new InitLevelState(_scenarioInstance, _roomContext, _statsProvider, _triggerFactory, _playerFactory,
                        _enemyFactory, _navigationFactory, _roomObjectsFactory, _levelManager, _levelContext, _roomBuilder, _cameraManager);
                    break;
                case TypeOfState.PlayerTurn:
                    state = new PlayerTurnState(_scenarioInstance, _roomContext);
                    break;
                case TypeOfState.EnemyTurn:
                    state = new EnemyTurnState(_scenarioInstance, _roomContext);
                    break;
                case TypeOfState.Interstitial:
                    state = new InterstitialState(_scenarioInstance, _roomContext, _cameraManager);
                    break;
                case TypeOfState.Pause:
                    state = new PauseState(_scenarioInstance);
                    break;
            }
            return state;
        }

        public IConditionState CreateConditionState(TypeOfConditionState type, ICharacterController controller)
        {
            IConditionState state = null;
            switch (type)
            {
                case TypeOfConditionState.InactiveState:
                    state = new InactiveState(controller);
                    break;
                case TypeOfConditionState.DeadState:
                    state = new DeadState(controller);
                    break;
                case TypeOfConditionState.StunState:
                    state = new StunState(controller);
                    break;
                case TypeOfConditionState.PlayerActiveState:
                    state = new PlayerActiveState(controller, _projectilePooler);
                    break;
                case TypeOfConditionState.EnemyActiveState:
                    state = new EnemyActiveState(controller, _projectilePooler);
                    break;
            }
            return state;
        }
    }
}
