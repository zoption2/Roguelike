using CharactersStats;
using UnityEngine;
using UnityEngine.TextCore.Text;

public interface IConditionState
{
    public void OnEnter();
    public void OnExit();
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

public class StunState : IConditionState
{
    private ICharacterController _characterController;

    public StunState(ICharacterController characterController)
    {
        _characterController = characterController;
    }
    public void OnEnter()
    {
        _characterController.IsStunned = true;
        Debug.Log("<color=#F43D39>" + "--|Enter Stun Condition State|-- " + "</color>");
        
    }

    public void OnExit()
    {
        _characterController.IsStunned = false;
        Debug.Log("<color=#F43D39>" + "--|Exit Stun Condition State|-- " + "</color>");
    }
}

