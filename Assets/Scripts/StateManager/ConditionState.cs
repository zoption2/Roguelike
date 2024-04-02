using CharactersStats;
using Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConditionState
{
    public void OnEnter();
    public void OnExit();
}

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
        } else
        {
            _controller.SwitchState(TypeOfConditionState.DefaultState);
        }
    }
}

public class DefaultState : IConditionState
{
    private ICharacterController _characterController;

    public DefaultState(ICharacterController characterController)
    {
        _characterController = characterController;
    }
    public void OnEnter()
    {
        Debug.Log("<color=#5A539C>" + "--|Enter Default Condition State|-- " + "</color>");
    }

    public void OnExit()
    {
        //Debug.Log("<color=#5A539C>" + "--|Exit Default Condition State|-- " + "</color>");
    }
}

public class DeadState : IConditionState
{
    private ICharacterController _characterController;

    public DeadState(ICharacterController characterController)
    {
        _characterController = characterController;
    }
    public void OnEnter()
    {
        Debug.Log("<color=#FFFFFF>" + "--|Enter Dead Condition State|-- " + "</color>");
        _characterController.PushIfDead();
    }

    public void OnExit()
    {
        //Debug.Log("<color=#FFFFFF>" + "--|Exit Dead Condition State|-- " + "</color>");
    }
}

