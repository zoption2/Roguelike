using Gameplay;
using NUnit.Framework;

[TestFixture]
public class ScenarioTests
{
    private DefaultScenario _scenario;
    private IStateFactory _stateFactory;

    [SetUp]
    public void SetUp()
    {
        _stateFactory = Create.StateFactory();
        _scenario = Create.Scenario(_stateFactory);
        Setup.ClearScenario(_scenario);
    }

    [Test]
    public void Scenario_OnTurnOrderCreate_QueueShouldContainFourStates()
    {
        // Arrange //
        var mockController1 = Create.MockCharacterController();
        var mockController2 = Create.MockCharacterController();

        var mapper1 = Create.Mapper(mockController1, TypeOfState.PlayerTurn);
        var mapper2 = Create.Mapper(mockController2, TypeOfState.EnemyTurn);

        var turnOrder = _scenario.GetTurnsOrder();
        turnOrder.Add(mapper1);
        turnOrder.Add(mapper2);

        Setup.StatesForScenario(_stateFactory);

        // Act //
        _scenario.RenewQueue();

        // Assert //
        var queueOfStates = _scenario.GetQueueOfStates();
        Assert.AreEqual(4, queueOfStates.Count, "The queue should contain four states.");
    }

    [Test]
    public void Scenario_OnTurnOrderCreate_QueueShouldHaveCorrectOrder()
    {
        // Arrange //
        var mockController1 = Create.MockCharacterController();
        var mockController2 = Create.MockCharacterController();

        var mapper1 = Create.Mapper(mockController1, TypeOfState.PlayerTurn);
        var mapper2 = Create.Mapper(mockController2, TypeOfState.EnemyTurn);

        var turnOrder = _scenario.GetTurnsOrder();
        turnOrder.Add(mapper1);
        turnOrder.Add(mapper2);

        Setup.StatesForScenario(_stateFactory);

        // Act //
        _scenario.RenewQueue();

        // Assert //
        var queueOfStates = _scenario.GetQueueOfStates();
        Assert.AreEqual(_stateFactory.CreateState(TypeOfState.PlayerTurn), queueOfStates.Dequeue(), "First state should be state1 (PlayerTurn).");
        Assert.AreEqual(_stateFactory.CreateState(TypeOfState.Interstitial), queueOfStates.Dequeue(), "Second state should be interstitialState.");
        Assert.AreEqual(_stateFactory.CreateState(TypeOfState.EnemyTurn), queueOfStates.Dequeue(), "Third state should be state2 (EnemyTurn).");
        Assert.AreEqual(_stateFactory.CreateState(TypeOfState.Interstitial), queueOfStates.Dequeue(), "Fourth state should be interstitialState.");
    }
}