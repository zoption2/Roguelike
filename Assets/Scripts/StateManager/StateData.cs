using CharactersStats;
using Pool;
using Zenject;
using Projectiles;
using UnityEngine;

namespace Gameplay
{
    public interface IStateFactory
    {
        public void Init(IScenario scenarioInstance, ICharacterScenarioContext context);
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
        private ICharacterScenarioContext _context;
        private INavigationFactory _navigationFactory;
        private RoomTemplateSO _roomTemplate;
        private ProjectilePooler _projectilePooler;

        [Inject]
        public void Construct(IBuffFactory triggerFactory ,IPlayerFactory playerFactory, IEnemyFactory enemyFactory, IStatsProvider statsProvider,
            INavigationFactory navigationFactory, ProjectilePooler projectilePooler, IRoomObjectsFactory roomObjectsFactory, RoomTemplateSO roomTemplateSO)
        {
            _triggerFactory = triggerFactory;
            _enemyFactory = enemyFactory;
            _playerFactory = playerFactory;
            _statsProvider = statsProvider;
            _navigationFactory = navigationFactory;
            _roomObjectsFactory = roomObjectsFactory;
            _roomTemplate = roomTemplateSO;
            _projectilePooler = projectilePooler;
            _projectilePooler.Init();
        }

        public void Init(IScenario scenarioInstance, ICharacterScenarioContext context)
        {
            _context = context;
            _scenarioInstance = scenarioInstance;
        }

        public IState CreateState(TypeOfState type)
        {
            IState state = null;
            switch (type)
            {
                case TypeOfState.Init:
                    state =  new InitLevelState(_scenarioInstance, _context, _statsProvider, _triggerFactory, _playerFactory,
                        _enemyFactory, _navigationFactory, _roomObjectsFactory, _roomTemplate);
                    break;
                case TypeOfState.PlayerTurn:
                    state = new PlayerTurnState(_scenarioInstance, _context);
                    break;
                case TypeOfState.EnemyTurn:
                    state = new EnemyTurnState(_scenarioInstance, _context);
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
                    state = new PlayerActiveState(controller,_projectilePooler);
                    break;
                case TypeOfConditionState.EnemyActiveState:
                    state = new EnemyActiveState(controller, _projectilePooler);
                    break;
            }
            return state;
        }
    }
}
