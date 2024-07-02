using Enemy;
using Gameplay;
using Player;
using Pool;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Zenject;
using System.Linq;

public interface IDefaultScenario : IScenario
{
    //public void LoadMainMenu();
}

public class DefaultScenario : Scenario<RoomContext>, IDefaultScenario
{

    public DefaultScenario(IGameplayService gameplayService, IStateFactory stateFactory)
    {
        GameplayService = gameplayService;
        _queueOfStates = new Queue<IState>();
        _turnsOrder = new List<CookedMapper>();
        _stateFactory = stateFactory;
    }
    public override void EraseCharacter(ICharacterController controller)
    {
        Debug.LogWarning($"EraseCharacter called for: {controller}");
        controller.ON_CHARACTER_DEATH -= EraseCharacter;
        controller.Dispose();

        _turnsOrder.RemoveAll(mapper => mapper.Controller == controller || mapper.Controller == null);

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

    public override void CheckConditonsForEndOfScenario()
    {
        bool noPlayers = _scenarioContext.Players.Count == 0;
        bool noEnemies = _scenarioContext.Enemies.Count == 0;

        if (noPlayers)
        {
            foreach (ICharacterController enemy in _scenarioContext.Enemies)
            {
                enemy.ON_CHARACTER_DEATH -= EraseCharacter;
                enemy.Dispose();
            }
            LoadMainMenu();
        }
        else if (noEnemies)
        {
            ActivateCompleatedRoomTriggers();

            //PREPAIR LOGIC TO MOVE PLAYER INTO ANOTHER SCENE!!!!!!

            //LoadMainMenu();
        }
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

    public override void LoadMainMenu()
    {
        GameplayService.PoolManager.CleanPoolers();
        SceneManager.LoadScene("Menu");
    }

    public void ActivateCompleatedRoomTriggers()
    {
        foreach (var trigger in _scenarioContext.CompleatedRoomTriggers)
        {
            trigger.ActivateTrigger();
        }
    }


    public override void Init(IScenarioContext context, LevelManager levelManager)
    {
        _levelManager = levelManager;
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
        if (_turnsOrder.Count == 0)
        {
            SubscribeToDeathOfCharacters();
            GetSortedTurns();
        }
        foreach (CookedMapper mapper in _turnsOrder)
        {
            IState state = _stateFactory.CreateState(mapper.State);
            state.SetCharacter(mapper.Controller);
            _queueOfStates.Enqueue(state);
        }
    }

}