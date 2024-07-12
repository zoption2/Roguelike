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

public interface IMainRoomScenario : IScenario
{ 

}


public class MainRoomScenario : Scenario<RoomContext>, IMainRoomScenario
{
    public MainRoomScenario(IGameplayService gameplayService, IStateFactory stateFactory, ILevelManager levelManager)
    {
        GameplayService = gameplayService;
        _queueOfStates = new Queue<IState>();
        _turnsOrder = new List<CookedMapper>();
        _stateFactory = stateFactory;
        _levelManager = levelManager;
    }

    public override void CheckConditonsForEndOfScenario()
    {
        bool noPlayers = _scenarioContext.Players.Count == 0;
        bool noEnemies = _scenarioContext.Enemies.Count == 0;

        Debug.Log("checking scenario end conditions...");
        if (noPlayers && !_haslost)
        {
            Debug.LogWarning("player lost");
            _haslost = true;
            ClearTurnOrder();
            LoadMainMenu();
        }
        else if (noEnemies && !_hasWon)
        {
            ActivateCompleatedRoomTriggers();
            UnlockAllChests();
            _hasWon = true;
            //PREPAIR LOGIC TO MOVE PLAYER INTO ANOTHER SCENE!!!!!!

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

        OnStateEnd();
    }

}
