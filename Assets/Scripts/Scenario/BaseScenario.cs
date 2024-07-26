using CharactersStats;
using Enemy;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Gameplay
{
    public abstract class Scenario<T> : IScenario where T : IRoomContext
    {
        protected IState _currentState;
        protected Queue<IState> _queueOfStates;
        protected T _scenarioContext;
        protected IStateFactory _stateFactory;
        protected ILevelManager _levelManager;
        protected List<CookedMapper> _turnsOrder;
        protected ILevelContext _levelContext;
        protected bool _haslost=false;
        protected bool _hasWon=false;
        protected IRewardService _rewardService;

        public IGameplayService GameplayService { get; set; }

        [Inject]
        public void Construct(ILevelContext levelContext, IRewardService rewardService)
        {
            _levelContext = levelContext;
            _rewardService = rewardService;
        }

        public void ClearTurnsOrder()
        {
            _turnsOrder.Clear();
        }

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
            Debug.Log("erasing the " + controller);
            controller.ON_CHARACTER_DEATH -= EraseCharacter;
            controller.Dispose();

            IState stateOfDeadCharacter = GetStateFromQueueByController(controller);

            RemoveCharacterFromTurnsOrder(controller);
            RemoveElementFromQueue(stateOfDeadCharacter);

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

            CheckConditonsForEndOfScenario();

            if (_queueOfStates.Count != 0)
            {
                IState state = _queueOfStates.Dequeue();
                SwitchState(state);
            }
            Debug.Log("OnStateEnd");
        }


        public void SwitchState(IState state)
        {
            Debug.Log("tried switching");
            if (_currentState != state)
            {
                Debug.Log("SwitchState");
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
            Debug.Log("load menu");
            ClearTurnOrder();
            ClearTurnQueue();
            _levelContext.CleanAllContexts();
            GameplayService.CurrentScenario = null;
            GameplayService.CurrentRoomName = null;
            GameplayService.CurrentRoomType = TypeOfScenario.MainRoom;
            _hasWon = false;
            _haslost = false;
            GameplayService.PoolManager.CleanPoolers();

            _levelManager.LoadMenu();
        }

        public void SubscribeToDeathOfCharacters()
        {
            foreach (ICharacterController controller in _scenarioContext.Players)
            {
                controller.ON_CHARACTER_DEATH -= EraseCharacter;
                controller.ON_CHARACTER_DEATH += EraseCharacter;
            }

            foreach (ICharacterController controller in _scenarioContext.Enemies)
            {
                controller.ON_CHARACTER_DEATH -= EraseCharacter;
                controller.ON_CHARACTER_DEATH += EraseCharacter;
            }
        }


        protected void SortTurns()
        {
            List<RawMapper> rawMappers = GetRawMappers();

            rawMappers = rawMappers.OrderByDescending(x => x.Speed).ToList();

            foreach (RawMapper mapper in rawMappers)
            {
                CookedMapper cookedMapper;
                cookedMapper = ConvertToCookedMapper(mapper.Controller);
                _turnsOrder.Add(cookedMapper);
            }
        }

        protected IState GetStateFromQueueByController(ICharacterController controller)
        {
            foreach (IState state in _queueOfStates)
            {
                if (state.GetCharacter() == controller)
                {
                    return state;
                }
            }
            return null;
        }

        protected void RemoveElementFromQueue(IState stateForRemoval)
        {

            Queue<IState> queue = new Queue<IState>();

            foreach(IState queueState in _queueOfStates)
            {
                if (queueState != stateForRemoval)
                {
                    queue.Enqueue(queueState);
                }
            }

            _queueOfStates = queue;
        }

        protected void ClearTurnOrder()
        {
            _turnsOrder.Clear();
        }

        protected void ClearTurnQueue()
        {
            _queueOfStates.Clear();
        }

        protected void AddToRawMappers(List<RawMapper> rawMappers, List<ICharacterController> controllers)
        {
            foreach (ICharacterController controller in controllers)
            {
                RawMapper mapper = new RawMapper();
                mapper.Controller = controller;
                mapper.Speed = controller.CharacterModel.Speed;
                rawMappers.Add(mapper);
            }
        }

        protected List<RawMapper> GetRawMappers()
        {
            List<RawMapper> rawMappers = new List<RawMapper>();

            AddToRawMappers(rawMappers, _scenarioContext.Players.Cast<ICharacterController>().ToList());

            AddToRawMappers(rawMappers, _scenarioContext.Enemies.Cast<ICharacterController>().ToList());

            return rawMappers;
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

        public void ActivateCompletedRoomTriggers()
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

    public class RawMapper
    {
        public int Speed;
        public ICharacterController Controller;

        public override string ToString()
        {
            return $"Controller: {Controller}, Speed: {Speed}";
        }
    }
}
