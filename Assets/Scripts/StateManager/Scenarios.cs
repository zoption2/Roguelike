using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Enemy;
using Prefab;
using Zenject;

namespace Gameplay
{
    public class BossScenario : Scenario
    {
        public override void Init()
        {
            //Init some stuff
        }

        public override void RenewQueue()
        {
            
        }

        public BossScenario(IGameplayService fullService)
        {
            _gameplayService = fullService;
        }
    }

    public class DefaultScenario : Scenario
    {
        public DefaultScenario(IGameplayService fullService)
        {
            _gameplayService = fullService;
            Debug.Log("This is the default scenario");
            _queueOfStates = new Queue<IState>();
            _stateSet = StateData.GetStateSet(this,_gameplayService);
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
    public abstract class Scenario
    {
        protected IState _currentState;
        protected Queue<IState> _queueOfStates;
        protected Dictionary<TypeOfState, IState> _stateSet;

        public IGameplayService _gameplayService;

        public abstract void RenewQueue();
        public IState GetCurrentState()
        {
            return _currentState;
        }
        public Scenario()
        {
            
        }
        public abstract void Init();


        public void OnStateEnd(/*no index needed because you can take only first element from queue*/)
        {
           //take next state from queue
           IState state = _queueOfStates.Dequeue();
           //switch current state to the taken from queue
           SwitchState(state);
           //if there is no more states(or other reasons)
            if (_queueOfStates.Count == 0)
            {
                // renew queue with new states
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
