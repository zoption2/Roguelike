using System.Collections.Generic;

namespace Gameplay
{
    public abstract class Scenario<T> : IScenario where T : IScenarioContext
    {
        protected IState _currentState;
        private List<CookedMapper> _turnsOrder;
        protected Queue<IState> _queueOfStates;
        protected T _scenarioContext;
        protected IStateFactory _stateFactory;
        protected LevelManager _levelManager;

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
        }
        public IState GetCurrentState()
        {
            return _currentState;
        }

        public abstract void Init(IScenarioContext context, LevelManager levelManager);

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
                SwitchState(state);
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

        public abstract void LoadMainMenu();

        public void CreateNewRoomScene(int level, int room)
        {
        }
    }
    public class CookedMapper
    {
        public ICharacterController Controller;
        public TypeOfState State;
    }
}