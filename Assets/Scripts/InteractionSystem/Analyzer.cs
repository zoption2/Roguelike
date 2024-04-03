using CharactersStats;
using UnityEngine;

public interface IAnalyzer
{
    public void Analyze(ReactiveStats stats);
}

public class Analyzer : IAnalyzer
{
    private ICharacterController _controller;
    public Analyzer(ICharacterController controller)
    {
        _controller = controller;
    }
    public void Analyze(ReactiveStats stats)
    {

        Debug.Log(stats.Health.Value);
        if (stats.Health.Value <= 0)
        {
            _controller.SwitchState(TypeOfConditionState.DeadState);
        }
        else
        {
            _controller.SwitchState(TypeOfConditionState.DefaultState);
        }
    }
}
