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


    public override void CheckConditonsForEndOfScenario()
    {
        bool noPlayers = _scenarioContext.Players.Count == 0;
        bool noEnemies = _scenarioContext.Enemies.Count == 0;

        if (noPlayers)
        {
            LoadMainMenu();
        }
        else if (noEnemies)
        {
            ActivateCompleatedRoomTriggers();

            //PREPAIRE LOGIC TO MOVE PLAYER INTO ANOTHER SCENE!!!!!!

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