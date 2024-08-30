using Gameplay;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class ScenarioTests
{
    private DefaultScenario _scenario;
    private IStateFactory _stateFactory;
    private ILevelContext _levelContext;
    private IRewardService _rewardService;
    private IGameplayService _gameplayService;
    private ILevelManager _levelManager;

    [SetUp]
    public void SetUp()
    {
        _stateFactory = Substitute.For<IStateFactory>();
        _levelContext = Substitute.For<ILevelContext>();
        _rewardService = Substitute.For<IRewardService>();
        _gameplayService = Substitute.For<IGameplayService>();
        _levelManager = Substitute.For<ILevelManager>();

        _scenario = new DefaultScenario(_gameplayService, _stateFactory, _levelManager);
        _scenario.Construct(_levelContext, _rewardService);

        _scenario.ClearTurnsOrder();
        _scenario.ClearTurnQueue();
    }

    [Test]
    public void Scenario_OnTurnOrderCreate_IsTheTurnOrderCorrect()
    {
        // Arrange //
        var mockController1 = Substitute.For<ICharacterController>();
        var mockController2 = Substitute.For<ICharacterController>();

        var mapper1 = new CookedMapper { Controller = mockController1, State = TypeOfState.PlayerTurn };
        var mapper2 = new CookedMapper { Controller = mockController2, State = TypeOfState.EnemyTurn };

        var turnOrder = _scenario.GetTurnsOrder();
        turnOrder.Add(mapper1);
        turnOrder.Add(mapper2);

        var state1 = Substitute.For<IState>();
        var state2 = Substitute.For<IState>();
        var interstitialState = Substitute.For<IState>();

        _stateFactory.CreateState(TypeOfState.PlayerTurn).Returns(state1);
        _stateFactory.CreateState(TypeOfState.EnemyTurn).Returns(state2);
        _stateFactory.CreateState(TypeOfState.Interstitial).Returns(interstitialState);

        // Act //
        _scenario.RenewQueue();

        // Assert //
        var queueOfStates = _scenario.GetQueueOfStates();
        Assert.AreEqual(4, queueOfStates.Count, "The queue should contain four states.");

        Assert.AreEqual(state1, queueOfStates.Dequeue(), "First state should be state1 (PlayerTurn).");
        Assert.AreEqual(interstitialState, queueOfStates.Dequeue(), "Second state should be interstitialState.");
        Assert.AreEqual(state2, queueOfStates.Dequeue(), "Third state should be state2 (EnemyTurn).");
        Assert.AreEqual(interstitialState, queueOfStates.Dequeue(), "Fourth state should be interstitialState.");
    }
}
