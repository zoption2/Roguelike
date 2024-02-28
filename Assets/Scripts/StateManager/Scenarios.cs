using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Enemy;
using Prefab;
using Zenject;

namespace Gameplay
{
    public class BossScenario : Scenario<DefaultScenarioContext>
    {
        public override void Init()
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

    public class DefaultScenario : Scenario<DefaultScenarioContext>
    {
        public DefaultScenario(IGameplayService fullService, IScenarioContext scenarioContext)
        {
            _gameplayService = fullService;
            Debug.Log("This is the default scenario");
            _queueOfStates = new Queue<IState>();
            SetScenarioContext(scenarioContext);
            _stateSet = StateData.GetStateSet(this,_scenarioContext);
        }
        public override void Init()
        {
            _queueOfStates.Enqueue(_stateSet[TypeOfState.Init]);
            _queueOfStates.Enqueue(_stateSet[TypeOfState.PlayerTurn]);
            _queueOfStates.Enqueue(_stateSet[TypeOfState.EnemyTurn]);

            _currentState = _queueOfStates.Dequeue();
            _currentState.OnEnter();
        }

        public override void RenewQueue()
        {
            _queueOfStates.Enqueue(_stateSet[TypeOfState.PlayerTurn]);
            _queueOfStates.Enqueue(_stateSet[TypeOfState.EnemyTurn]);
        }
    }

    public interface IScenario
    {
        object GetScenarioContext();
        public void OnStateEnd();
        public void Init();
        public IGameplayService _gameplayService { get; set; }
    }
    public abstract class Scenario<T> : IScenario where T: IScenarioContext
    {
        protected IState _currentState;
        protected Queue<IState> _queueOfStates;
        protected Dictionary<TypeOfState, IState> _stateSet;
        protected T _scenarioContext;

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
        public abstract void Init();

        public void OnStateEnd()
        {
           IState state = _queueOfStates.Dequeue();
           SwitchState(state);
            if (_queueOfStates.Count == 0)
            {
                RenewQueue();
            }
           
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
}
