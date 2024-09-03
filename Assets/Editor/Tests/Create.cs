using Gameplay;
using NSubstitute;

public static class Create
{
    public static DefaultScenario Scenario(IStateFactory stateFactory)
    {
        var gameplayService = Substitute.For<IGameplayService>();
        var levelManager = Substitute.For<ILevelManager>();

        var scenario = new DefaultScenario(gameplayService, stateFactory, levelManager);

        var levelContext = Substitute.For<ILevelContext>();
        var rewardService = Substitute.For<IRewardService>();
        scenario.Construct(levelContext, rewardService);

        return scenario;
    }

    public static IStateFactory StateFactory()
    {
        return Substitute.For<IStateFactory>();
    }

    public static ICharacterController MockCharacterController()
    {
        return Substitute.For<ICharacterController>();
    }

    public static CookedMapper Mapper(ICharacterController controller, TypeOfState state)
    {
        return new CookedMapper { Controller = controller, State = state };
    }
}

