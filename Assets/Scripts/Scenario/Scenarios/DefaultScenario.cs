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

    public DefaultScenario(IGameplayService gameplayService, IStateFactory stateFactory, ILevelManager levelManager)
    {
        GameplayService = gameplayService;
        _queueOfStates = new Queue<IState>();
        _turnsOrder = new List<CookedMapper>();
        _stateFactory = stateFactory;
        _levelManager = levelManager;
    }


    public override void CheckConditonsForEndOfScenario()
    {
        foreach (var obj in _turnsOrder)
        {
            Debug.LogWarning(obj.ToString());
        }

        Debug.LogError(_scenarioContext.Players.Count);
        Debug.LogError(_scenarioContext.Enemies.Count);

        bool noPlayers = _scenarioContext.Players.Count == 0;
        bool noEnemies = _scenarioContext.Enemies.Count == 0;

        Debug.Log("checking scenario end conditions...");
        if (noPlayers && !_haslost)
        {
            Debug.LogWarning("player lost in defaultScenario");
            _haslost = true;
            ClearTurnOrder();
            ClearTurnQueue();
            LoadMainMenu();
        }
        else if (noEnemies && !_hasWon)
        {
            ActivateCompletedRoomTriggers();
            _hasWon = true;

            //LoadMainMenu();
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

        SubscribeToDeathOfCharacters();
        SortTurns();

        OnStateEnd();
    }
}