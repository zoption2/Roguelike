using CharactersStats;
using Enemy;
using Interactions;
using Player;

public interface IAnalyzer
{
    public void Analyze(ReactiveStats stats, IEffectProcessor effectProcessor);
}

public class Analyzer : IAnalyzer
{
    private ICharacterController _controller;
    public Analyzer(ICharacterController controller)
    {
        _controller = controller;
    }
    public void Analyze(ReactiveStats stats, IEffectProcessor effectProcessor)
    { 
        if (stats.Health.Value <= 0)
        {
            _controller.SwitchState(TypeOfConditionState.DeadState);
            return;
        }

        foreach (IEffect effect in effectProcessor.GetOnStartTurnInteractionEffects())
        {
            if (effect is StunEffect)
            {
                _controller.SwitchState(TypeOfConditionState.StunState);
                return;
            }
        }

        if (_controller.IsActive)
        {
            if (_controller is IPlayerController)
            {
                _controller.SwitchState(TypeOfConditionState.PlayerActiveState);
                return;
            }
            else if (_controller is IEnemyController)
            {
                _controller.SwitchState(TypeOfConditionState.EnemyActiveState);
                return;
            }
        }
        else if (!_controller.IsActive)
        {
            _controller.SwitchState(TypeOfConditionState.InactiveState);
            return;
        }
    }
}
