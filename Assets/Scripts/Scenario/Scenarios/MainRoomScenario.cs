using Enemy;
using Gameplay;
using Player;
using Pool;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IChestScenario : IScenario
{ 

}


public class ChestScenario : Scenario<RoomContext>, IChestScenario
{
    public ChestScenario(IGameplayService gameplayService, IStateFactory stateFactory, ILevelManager levelManager)
    {
        GameplayService = gameplayService;
        _queueOfStates = new Queue<IState>();
        _turnsOrder = new List<CookedMapper>();
        _stateFactory = stateFactory;
        _levelManager = levelManager;
    }

    public override void CheckConditonsForEndOfScenario()
    {
        Debug.LogError(_scenarioContext.Players.Count);
        Debug.LogError(_scenarioContext.Enemies.Count);

        bool noPlayers = _scenarioContext.Players.Count == 0;
        bool noEnemies = _scenarioContext.Enemies.Count == 0;

        Debug.Log("checking scenario end conditions...");
        if (noPlayers && !_haslost)
        {
            Debug.LogWarning("player lost in mainScenario");
            _haslost = true;
            ClearTurnOrder();
            ClearTurnQueue();
            LoadMainMenu();
        }
        else if (noEnemies && !_hasWon)
        {
            ActivateCompletedRoomTriggers();
            UnlockAllChests();
            _hasWon = true;

            //LoadMainMenu();
        }
    }

    private void UnlockAllChests()
    {
        foreach(IChestController chest in _scenarioContext.Chests)
        {
            chest.UnlockChest();
        }
        Debug.Log("unlocked all chests!");
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

        OnStateEnd(); //??
    }

}
