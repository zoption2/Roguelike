using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public abstract class Scenario<T> : IScenario where T : IScenarioContext
    {
        protected IState _currentState;
        private List<CookedMapper> _turnsOrder;
        protected Queue<IState> _queueOfStates;
        protected T _scenarioContext;
        protected IStateFactory _stateFactory;
        protected List<CookedMapper> _turnsOrder;

        public IGameplayService GameplayService { get; set; }

        public abstract void RenewQueue();
        public abstract void CheckConditonsForEndOfScenario();

        public object GetScenarioContext()
        {
            return _scenarioContext;
        }

        public void SetScenarioContext(IScenarioContext context)
        {
            _scenarioContext = (T)context;
            Debug.Log("Scenario context set: " + _scenarioContext);
        }

        public IState GetCurrentState()
        {
            return _currentState;
        }

        public abstract void Init(IScenarioContext context);

        public abstract void EraseCharacter(ICharacterController controller);

        public void OnStateEnd()
        {
            if (_queueOfStates.Count == 0)
            {
                RenewQueue();
            }

            if (_queueOfStates.Count != 0)
            {
                IState state = _queueOfStates.Dequeue();
                Debug.Log("Dequeue state: " + state);
                SwitchState(state);
            }
        }


        public void SwitchState(IState state)
        {
            if (_currentState != state)
            {
                Debug.Log("Switching state from " + _currentState + " to " + state);
                _currentState?.OnExit();
                _currentState = state;
                _currentState.OnEnter();
            }
        }

        public void Pause()
        {
            IState state = _stateFactory.CreateState(TypeOfState.Pause);
            //_queueOfStates.Enqueue(state);
            SwitchState(state);
        }

        public abstract void LoadMainMenu();

        public void CreateNewRoomScene(int level, int room)
        {

        }

        public void RemoveDeadCharactersFromQueue()
        {

            _turnsOrder = _turnsOrder.Where(mapper => mapper.Controller != null && !mapper.Controller.IsDead).ToList();
        }
    }

    public class CookedMapper
    {
        public ICharacterController Controller;
        public TypeOfState State;

        public override string ToString()
        {
            return $"Controller: {Controller}, State: {State}";
        }
    }
}
