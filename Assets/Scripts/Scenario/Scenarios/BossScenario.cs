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

    }

    public override void EraseCharacter(ICharacterController controller)
    {
        base.EraseCharacter(controller);
        CheckConditonsForEndOfScenario();
    }
}