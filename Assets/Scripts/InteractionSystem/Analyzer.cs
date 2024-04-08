using CharactersStats;
using Interactions;
using UnityEngine;

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

        Debug.Log("Health on start: " + stats.Health.Value);
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

        _controller.SwitchState(TypeOfConditionState.DefaultState);
    }
}
