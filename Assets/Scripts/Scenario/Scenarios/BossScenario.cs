using Gameplay;

public class BossScenario : Scenario<DefaultScenarioContext>
{
    public override void Init(IScenarioContext context, LevelManager levelManager)
    {
        _levelManager = levelManager;
    }

    public override void RenewQueue()
    {

    }

    public BossScenario(IGameplayService fullService)
    {
        _gameplayService = fullService;
    }

    public override void CheckConditonsForEndOfScenario()
    {

    }

    public override void LoadMainMenu()
    {
    }

    public override void EraseCharacter(ICharacterController controller)
    {
    }
}