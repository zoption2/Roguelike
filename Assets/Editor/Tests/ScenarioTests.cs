using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;
using Gameplay;
using Zenject;

public class ScenarioTests
{
    private Scenario<IRoomContext> _scenario;
    private IStateFactory _stateFactory;
    private ILevelContext _levelContext;
    private IRewardService _rewardService;

    [SetUp]
    public void SetUp()
    {
        _stateFactory = Substitute.For<IStateFactory>();
        _levelContext = Substitute.For<ILevelContext>();
        _rewardService = Substitute.For<IRewardService>();

        _scenario = Substitute.ForPartsOf<TestScenario>();
        _scenario.Construct(_levelContext, _rewardService);
        //_scenario.SetStateFactory(_stateFactory);

        _scenario.ClearTurnOrder();
        _scenario.ClearTurnQueue();
    }

    private class TestScenario : Scenario<IRoomContext>
    {
        public override void CheckConditonsForEndOfScenario() { }
        public override void Init(IScenarioContext context) { }
    }

    [Test]
    public void RenewQueue_PopulatesQueueBasedOnTurnsOrder()
    {

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

        _scenario.RenewQueue();

        var queueOfStates = _scenario.GetQueueOfStates();
        Assert.AreEqual(4, queueOfStates.Count);

        Assert.AreEqual(state1, queueOfStates.Dequeue());
        Assert.AreEqual(interstitialState, queueOfStates.Dequeue());
        Assert.AreEqual(state2, queueOfStates.Dequeue());
        Assert.AreEqual(interstitialState, queueOfStates.Dequeue());
    }

}
