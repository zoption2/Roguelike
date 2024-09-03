using Gameplay;
using NSubstitute;

public static class Setup
{
    public static void ClearScenario(DefaultScenario scenario)
    {
        scenario.ClearTurnsOrder();
        scenario.ClearTurnQueue();
    }

    public static void StatesForScenario(IStateFactory stateFactory)
    {
        var state1 = Substitute.For<IState>();
        var state2 = Substitute.For<IState>();
        var interstitialState = Substitute.For<IState>();

        stateFactory.CreateState(TypeOfState.PlayerTurn).Returns(state1);
        stateFactory.CreateState(TypeOfState.EnemyTurn).Returns(state2);
        stateFactory.CreateState(TypeOfState.Interstitial).Returns(interstitialState);
    }
}
