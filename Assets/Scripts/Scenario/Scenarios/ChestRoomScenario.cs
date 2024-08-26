using Gameplay;
using System.Collections.Generic;
using UnityEngine;

public interface IChestScenario : IScenario
{

}


public class ChestRoomScenario : Scenario<RoomContext>, IChestScenario
{
    public ChestRoomScenario(IGameplayService gameplayService, IStateFactory stateFactory, ILevelManager levelManager)
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

        if (noPlayers && !_haslost)
        {
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
        foreach (IChestController chest in _scenarioContext.Chests)
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
