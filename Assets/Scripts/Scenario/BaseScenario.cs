using Enemy;
using Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public abstract class Scenario<T> : IScenario where T : IRoomContext
    {
        protected bool _haslost = false;
        protected bool _hasWon = false;
        protected List<CookedMapper> _turnsOrder;
        protected Queue<IState> _queueOfStates;
        protected IState _currentState;
        protected T _scenarioContext;
        protected IStateFactory _stateFactory;
        protected ILevelManager _levelManager;
        protected ILevelContext _levelContext;
        protected IRewardService _rewardService;

        public IGameplayService GameplayService { get; set; }

        [Inject]
        public void Construct(ILevelContext levelContext, IRewardService rewardService)
        {
            _levelContext = levelContext;
            _rewardService = rewardService;
        }

        public List<CookedMapper> GetTurnsOrder()
        {
            return _turnsOrder;
        }

        public Queue<IState> GetQueueOfStates()
        {
            return _queueOfStates;
        }

        public IState GetCurrentState()
        {
            return _currentState;
        }

        public void ClearTurnsOrder()
        {
            _turnsOrder.Clear();
        }

        public void RenewQueue()
        {
            _queueOfStates.Clear();

            for (int i = 0; i < _turnsOrder.Count; i++)
            {
                CookedMapper currentMapper = _turnsOrder[i];

                IState state = _stateFactory.CreateState(currentMapper.State);
                state.SetCharacter(currentMapper.Controller);
                _queueOfStates.Enqueue(state);

                if (_turnsOrder.Count > 1)
                {
                    CookedMapper nextMapper = _turnsOrder[(i + 1) % _turnsOrder.Count];
                    IState interstitialState = _stateFactory.CreateState(TypeOfState.Interstitial);
                    interstitialState.SetCharacter(nextMapper.Controller);
                    _queueOfStates.Enqueue(interstitialState);
                }
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
            RenewQueue();
        }

        public object GetScenarioContext()
        {
            return _scenarioContext;
        }

        public void SetScenarioContext(IScenarioContext context)
        {
            _scenarioContext = (T)context;
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

            CheckConditonsForEndOfScenario();
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
                SwitchState(state);
            }

            CheckConditonsForEndOfScenario();
        }

        public void SwitchState(IState state)
        {
            if (_currentState != state)
            {
                _currentState?.OnExit();
                _currentState = state;
                _currentState.OnEnter();
                Debug.Log("Switched turn State");
            }
        }

        public void HandleRoomChange()
        {
            _currentState.OnExit();
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
            GameplayService.ON_END_TURN -= this.OnStateEnd;
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

            foreach (IState queueState in _queueOfStates)
            {
                if (queueState != stateForRemoval)
                {
                    queue.Enqueue(queueState);
                }
            }

            _queueOfStates = queue;
        }

        public void ClearTurnOrder()
        {
            _turnsOrder.Clear();
        }

        public void ClearTurnQueue()
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
