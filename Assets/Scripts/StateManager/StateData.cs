using CharactersStats;
using Zenject;

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
        private IPlayerFactory _playerFactory;
        private IEnemyFactory _enemyFactory;
        private IStatsProvider _statsProvider;
        private IScenario _scenarioInstance;
        private ICharacterScenarioContext _context;
        private INavigationFactory _navigationFactory;

        [Inject]
        public void Construct(IPlayerFactory playerFactory, IEnemyFactory enemyFactory, IStatsProvider statsProvider,
            INavigationFactory navigationFactory)
        {
            _enemyFactory = enemyFactory;
            _playerFactory = playerFactory;
            _statsProvider = statsProvider;
            _navigationFactory = navigationFactory;
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
                    state =  new InitLevelState(_scenarioInstance, _context, _statsProvider, _playerFactory,
                        _enemyFactory, _navigationFactory);
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
                    state = new PlayerActiveState(controller);
                    break;
                case TypeOfConditionState.EnemyActiveState:
                    state = new EnemyActiveState(controller);
                    break;
            }
            return state;
        }
    }
}
