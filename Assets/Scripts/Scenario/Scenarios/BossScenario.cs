using Gameplay;

public class BossScenario : Scenario<RoomContext>
{
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

    public BossScenario(IGameplayService fullService, IStateFactory stateFactory)
    {
        GameplayService = fullService;
        _stateFactory = stateFactory;
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
            _hasWon = true;
        }
    }

}