using Enemy;
using Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public abstract class Scenario<T> : IScenario where T : IRoomContext
    {
        protected IState _currentState;
        protected Queue<IState> _queueOfStates;
        protected T _scenarioContext;
        protected IStateFactory _stateFactory;
        protected List<CookedMapper> _turnsOrder;

        public IGameplayService GameplayService { get; set; }

        public void RenewQueue()
        {
            foreach (CookedMapper mapper in _turnsOrder)
            {
                IState state = _stateFactory.CreateState(mapper.State);
                state.SetCharacter(mapper.Controller);
                _queueOfStates.Enqueue(state);
            }
        }
        public abstract void CheckConditonsForEndOfScenario();

        protected void RemoveCharacterFromTurnsOrder(ICharacterController controller)
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

        protected void RemoveCharacterFromDataTransfer(ICharacterController controller)
        {
            foreach (RawMapper mapper in DataTransfer.RawMappers)
            {
                if (mapper.Controller == controller)
                {
                    DataTransfer.RawMappers.Remove(mapper);
                    break;
                }
            }
        }

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

        public virtual void EraseCharacter(ICharacterController controller)
        {
            controller.ON_CHARACTER_DEATH -= EraseCharacter;
            controller.Dispose();

            RemoveCharacterFromTurnsOrder(controller);
            RemoveCharacterFromDataTransfer(controller);

            if (controller is IPlayerController)
            {
                _scenarioContext.Players.Remove((IPlayerController)controller);
            }
            else
            {
                _scenarioContext.Enemies.Remove((IEnemyController)controller);
            }

        }

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
                CheckConditonsForEndOfScenario();
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

        public void LoadMainMenu()
        {
            GameplayService.PoolManager.CleanPoolers();
            SceneManager.LoadScene("Menu");
        }

        public void SubscribeToDeathOfCharacters()
        {
            foreach (ICharacterController controller in _scenarioContext.Players)
            {
                controller.ON_CHARACTER_DEATH -= EraseCharacter;
                controller.ON_CHARACTER_DEATH += EraseCharacter;
                Debug.Log($"Subscribed to ON_CHARACTER_DEATH for player: {controller}");
            }

            foreach (ICharacterController controller in _scenarioContext.Enemies)
            {
                controller.ON_CHARACTER_DEATH -= EraseCharacter;
                controller.ON_CHARACTER_DEATH += EraseCharacter;
                Debug.Log($"Subscribed to ON_CHARACTER_DEATH for enemy: {controller}");
            }
        }

        protected void SortTurns()
        {
            DataTransfer.RawMappers = DataTransfer.RawMappers.OrderByDescending(x => x.Speed).ToList();
            foreach (RawMapper mapper in DataTransfer.RawMappers)
            {
                CookedMapper cookedMapper;
                cookedMapper = ConvertToCookedMapper(mapper.Controller);
                _turnsOrder.Add(cookedMapper);
            }
        }

        protected CookedMapper ConvertToCookedMapper(ICharacterController characterController)
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

        public void ActivateCompleatedRoomTriggers()
        {
            foreach (var trigger in _scenarioContext.CompleatedRoomTriggers)
            {
                trigger.ActivateTrigger();
            }
        }

        public void CreateNewRoomScene(int level, int room)
        {

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
