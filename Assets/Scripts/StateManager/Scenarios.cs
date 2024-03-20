using Enemy;
using System.Collections.Generic;
using UnityEngine;


namespace Gameplay
{
    public class BossScenario : Scenario<DefaultScenarioContext>
    {
        public override void Init(IScenarioContext context)
        {

        }

        public override void RenewQueue()
        {

        }

        public BossScenario(IGameplayService fullService, IScenarioContext scenarioContext)
        {
            _gameplayService = fullService;
        }
    }

    public class DefaultScenario : Scenario<DefaultScenarioContext>, IDefaultScenario
    {
        private List<CookedMapper> _turnsOrder;
        public DefaultScenario(IGameplayService gameplayService, IStateFactory stateFactory)
        {
            _gameplayService = gameplayService;
            _queueOfStates = new Queue<IState>();
            _turnsOrder = new List<CookedMapper>();
            _stateFactory = stateFactory;
        }
        public void EraseCharacter(ICharacterController controller)
        {
            foreach (CookedMapper mapper in _turnsOrder)
            {
                if (mapper.Controller == controller)
                {
                    _turnsOrder.Remove(mapper);
                    break;
                }
            }
        }

        
        public override void Init(IScenarioContext context)
        {
            SetScenarioContext(context);
            _stateFactory.Init(this, _scenarioContext);
            IState state = _stateFactory.CreateState(TypeOfState.Init);
            _queueOfStates.Enqueue(state);
            _currentState = _queueOfStates.Dequeue();
            _currentState.OnEnter();
        }

        private void GetSortedTurns()
        {
            DataTransfer.RawMappers.Sort();
            DataTransfer.RawMappers.Reverse();
            foreach (RawMapper mapper in DataTransfer.RawMappers)
            {
                CookedMapper cookedMapper;
                cookedMapper = ConvertToCookedMapper(mapper.Controller);
                _turnsOrder.Add(cookedMapper);
                Debug.Log("speed: " + mapper.Speed);
            }
        }

        private void DebugControllerCheck()
        {
            for (int i = 0, n = _turnsOrder.Count; i < n-1; i++)
            {
                for (int j = i+1; j < n; j++)
                {
                    Debug.Log($"Controllers {i} and {j} are equal: " + (_turnsOrder[i].Controller == _turnsOrder[j].Controller));
                }
            }
        }


        private CookedMapper ConvertToCookedMapper(ICharacterController characterController)
        {
            CookedMapper cookedMapper = new CookedMapper();
            cookedMapper.Controller = characterController;
            if (cookedMapper.Controller is IEnemyController)
            {
                cookedMapper.State = TypeOfState.EnemyTurn;
            }
            else
            {
                cookedMapper.State = TypeOfState.PlayerTurn;
            }
            return cookedMapper;
        }
        public override void RenewQueue()
        {
            if(_turnsOrder.Count == 0)
            {
                GetSortedTurns();
                //DebugControllerCheck();
            }
            foreach(CookedMapper mapper in _turnsOrder)
            {
                IState state = _stateFactory.CreateState(mapper.State);
                state.SetCharacter(mapper.Controller);
                _queueOfStates.Enqueue(state);
            }
        }
    }

    public interface IScenario
    {
        object GetScenarioContext();
        public void OnStateEnd();
        public void Init(IScenarioContext context);
        public IGameplayService _gameplayService { get; set; }
    }

    public interface IDefaultScenario : IScenario
    {

    }

    public abstract class Scenario<T> : IScenario where T : IScenarioContext
    {
        protected IState _currentState;
        protected Queue<IState> _queueOfStates;
        protected T _scenarioContext;
        protected IStateFactory _stateFactory;

        public IGameplayService _gameplayService { get; set; }


        public abstract void RenewQueue();

        public object GetScenarioContext()
        {
            return _scenarioContext;
        }
        public void SetScenarioContext(IScenarioContext context)
        {
            _scenarioContext = (T)context;
        }
        public IState GetCurrentState()
        {
            return _currentState;
        }
        public abstract void Init(IScenarioContext context);

        public void OnStateEnd()
        {
            if (_queueOfStates.Count == 0)
            {
                RenewQueue();
            }
            IState state = _queueOfStates.Dequeue();
            SwitchState(state);
            Debug.Log("TurnChanged");
        }
        public void SwitchState(IState state)
        {
            if (_currentState != state)
            {
                _currentState?.OnExit();
                _currentState = state;
                _currentState.OnEnter();
            }
        }
    }
    public class CookedMapper
    {
        public ICharacterController Controller;
        public TypeOfState State;
    }
}