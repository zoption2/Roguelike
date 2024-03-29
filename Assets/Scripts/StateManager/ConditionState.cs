using Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConditionState
{
    ICharacterController _controller { get; }
    public void OnEnter();
    public void OnExit();
}
public class IdleState : IConditionState
{

    public ICharacterController _controller { get; }

    public void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        throw new System.NotImplementedException();
    }

}
