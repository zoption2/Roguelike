using Enemy;
using Player;
using Pool;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public override void CheckConditonsForEndOfScenario()
        {

        }
    }

    public class DefaultScenario : Scenario<DefaultScenarioContext>, IDefaultScenario
    {
        private List<CookedMapper> _turnsOrder;
        private CharacterPooler _characterPooler;
        private CharacterUIPooler _characterUIPooler;
        public DefaultScenario(IGameplayService gameplayService, IStateFactory stateFactory, CharacterPooler characterPooler,
            CharacterUIPooler characterUIPooler)
        {
            _gameplayService = gameplayService;
            _queueOfStates = new Queue<IState>();
            _turnsOrder = new List<CookedMapper>();
            _stateFactory = stateFactory;
            _characterPooler = characterPooler;
            _characterUIPooler = characterUIPooler;
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

            if(controller is IPlayerController)
            {
                _scenarioContext.Players.Remove((IPlayerController)controller);
            }
            else
            {
                _scenarioContext.Enemies.Remove((IEnemyController)controller);
            }
            CheckConditonsForEndOfScenario();
        }
        
        private void CleanPoolers()
        {
            _characterPooler.CleanPool();
            _characterUIPooler.CleanPool();
            Debug.LogWarning("cleaned poolers!");
        }

        public override void CheckConditonsForEndOfScenario()
        {
            bool noPlayers = _scenarioContext.Players.Count == 0;
            bool noEnemies = _scenarioContext.Enemies.Count == 0;
            if (noPlayers || noEnemies)
            {
                if(noPlayers)
                {
                    LoadMainMenu();
                } else
                {
                    ActivateCompleatedRoomTriggers();
                }
                //if(noPlayers)
                //{
                //    foreach(ICharacterController enemy in _scenarioContext.Enemies)
                //    {
                //        enemy.JustPush();
                //    }
                //    Debug.LogWarning("You lost!");
                //}
                //else
                //{
                //    foreach (ICharacterController player in _scenarioContext.Players)
                //    {
                //        player.JustPush();
                //    }
                //    Debug.LogWarning("You won!");
                //}
                
                //LoadMainMenu();
            }
        }

        public void LoadMainMenu()
        {
            CleanPoolers();
            SceneManager.LoadScene("Menu");
        }

        public void ActivateCompleatedRoomTriggers()
        {
            foreach(var trigger in _scenarioContext.CompleatedRoomTriggers)
            {
                trigger.Activate();
            }
        }

        private void SubscribeToDeathOfCharacters()
        {
            foreach(ICharacterController controller in _scenarioContext.Players)
            {
                controller.ON_CHARACTER_DEATH += EraseCharacter;
            }

            foreach (ICharacterController controller in _scenarioContext.Enemies)
            {
                controller.ON_CHARACTER_DEATH += EraseCharacter;
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
            DataTransfer.RawMappers = DataTransfer.RawMappers.OrderByDescending(x => x.Speed).ToList();
            foreach (RawMapper mapper in DataTransfer.RawMappers)
            {
                CookedMapper cookedMapper;
                cookedMapper = ConvertToCookedMapper(mapper.Controller);
                _turnsOrder.Add(cookedMapper);
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
                SubscribeToDeathOfCharacters();
                GetSortedTurns();
            }
            foreach(CookedMapper mapper in _turnsOrder)
            {
                IState state = _stateFactory.CreateState(mapper.State);
                state.SetCharacter(mapper.Controller);
                _queueOfStates.Enqueue(state);
            }
            _scenarioContext.TeleportWallEnters.ForEach(teleportWallEnter => teleportWallEnter.Recharge());///
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
        public void LoadMainMenu();
    }

    public abstract class Scenario<T> : IScenario where T : IScenarioContext
    {
        protected IState _currentState;
        protected Queue<IState> _queueOfStates;
        protected T _scenarioContext;
        protected IStateFactory _stateFactory;

        public IGameplayService _gameplayService { get; set; }


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
        public abstract void Init(IScenarioContext context);

        public void OnStateEnd()
        {
            if (_queueOfStates.Count == 0)
            {
                RenewQueue();
            }
            IState state = _queueOfStates.Dequeue();
            SwitchState(state);
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