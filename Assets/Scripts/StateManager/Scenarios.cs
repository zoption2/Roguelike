using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Enemy;
using Prefab;
using Zenject;

namespace Gameplay
{
    public abstract class BaseScenario<T> where T : ISceneContextMarker
    {
        protected T _sceneContext;
        protected IState _currentState;
        protected Queue<IState> _queueOfStates;
        protected Dictionary<TypeOfState, IState> _stateSet;

        public abstract void RenewQueue();

        public IState GetCurrentState()
        {
            return _currentState;
        }

        public abstract void Init(ISceneContextMarker sceneContext);
        public abstract void FillTheQueue(Dictionary<TypeOfState, IState> dictionary);

        public BaseScenario()
        {
            _queueOfStates = new Queue<IState>();
            _stateSet = StateData.GetStateSet(this);
        }

        public void SetSceneContext(ISceneContextMarker context)
        {
            _sceneContext = (T)context;
        }

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

    //public class BossScenario : Scenario
    //{
    //    private MySceneContext _testSceneContext;
    //    public override void Init()
    //    {
    //        //Init some stuff
    //    }

    //    public override void Init(ISceneContextMarker sceneContext)
    //    {
    //        _testSceneContext = (MySceneContext)sceneContext;
    //    }

    //    public override void RenewQueue()
    //    {
            
    //    }

    //    public override void FillTheQueue(Dictionary<TypeOfState, IState> dictionary)
    //    {

    //    }

    //    public BossScenario(IGameplayService fullService)
    //    {
    //        _gameplayService = fullService;
    //    }
    //}

    public class TestScenario : BaseScenario<MySceneContext> 
    {
        public TestScenario()
        {
            Debug.Log("This is the default scenario");
            _queueOfStates = new Queue<IState>();
            _stateSet = StateData.GetStateSet(this);
        }

        public override void FillTheQueue(Dictionary<TypeOfState, IState> dictionary)
        {
            throw new System.NotImplementedException();
        }

        public override void Init(ISceneContextMarker sceneContext)
        {
            throw new System.NotImplementedException();
        }

        public override void RenewQueue()
        {
            throw new System.NotImplementedException();
        }
    }


    //public class DefaultScenario : Scenario
    //{
    //    private MySceneContext _testSceneContext;
    //    public DefaultScenario(IGameplayService fullService)
    //    {
    //        _gameplayService = fullService;
    //        Debug.Log("This is the default scenario");
    //        _queueOfStates = new Queue<IState>();
    //        _stateSet = StateData.GetStateSet(this,_gameplayService);
    //    }
    //    public override void Init()
    //    {
    //        _queueOfStates.Enqueue(_stateSet[TypeOfState.Init]);
    //        _queueOfStates.Enqueue(_stateSet[TypeOfState.PlayerTurn]);
    //        _queueOfStates.Enqueue(_stateSet[TypeOfState.EnemyTurn]);
    //        //FillTheQueue(_stateSet);

    //        _currentState = _queueOfStates.Dequeue();
    //        _currentState.OnEnter();
    //    }

    //    public override void Init(ISceneContextMarker sceneContext)
    //    {
    //        _testSceneContext = (MySceneContext)sceneContext;
    //    }

    //    public override void RenewQueue()
    //    {
    //        _queueOfStates.Enqueue(_stateSet[TypeOfState.PlayerTurn]);
    //        _queueOfStates.Enqueue(_stateSet[TypeOfState.EnemyTurn]);
    //    }

    //    public override void FillTheQueue(Dictionary<TypeOfState, IState> dictionary)
    //    {
    //        foreach (var state in dictionary)
    //        {
    //            _queueOfStates.Enqueue(state.Value);
    //        }
    //    }
    //}
    //public abstract class Scenario
    //{
    //    protected IState _currentState;
    //    protected Queue<IState> _queueOfStates;
    //    protected Dictionary<TypeOfState, IState> _stateSet;

    //    public IGameplayService _gameplayService;

    //    public abstract void RenewQueue();
    //    public IState GetCurrentState()
    //    {
    //        return _currentState;
    //    }
    //    public Scenario()
    //    {
            
    //    }
    //    public abstract void Init();
    //    public abstract void Init(ISceneContextMarker sceneContext);

    //    public abstract void FillTheQueue(Dictionary<TypeOfState, IState> dictionary);
    //    public void OnStateEnd(/*no index needed because you can take only first element from queue*/)
    //    {
    //       //take next state from queue
    //       IState state = _queueOfStates.Dequeue();
    //       //switch current state to the taken from queue
    //       SwitchState(state);
    //       //if there is no more states(or other reasons)
    //        if (_queueOfStates.Count == 0)
    //        {
    //            // renew queue with new states
    //            RenewQueue();
    //        }
           
    //    }
    //    public void SwitchState(IState state)
    //    {
    //        if (_currentState != state)
    //        {
    //            _currentState?.OnExit();
    //            _currentState = state;
    //            _currentState.OnEnter();
    //        }
    //    }
    //}
}
