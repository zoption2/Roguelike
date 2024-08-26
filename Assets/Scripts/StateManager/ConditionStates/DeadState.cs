using Interactions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeadState : IConditionState
{
    private ICharacterController _characterController;

    public DeadState(ICharacterController characterController)
    {
        _characterController = characterController;
    }

    public void DoUpdate()
    {
    }

    public void ApplyInteraction(IInteraction interaction)
    {
    }

    public IInteraction GetInteraction()
    {
        IInteraction interaction = _characterController.InteractionDealer.UseInteraction(InteractionType.None);
        return interaction;
    }

    public void LaunchYourself(Vector3 direction)
    {
    }

    public void OnEnter()
    {
        Debug.Log("<color=#FFFFFF>" + "--|Enter Dead Condition State|-- " + "</color>");
        if (_characterController.IsMoving)
        {
            _characterController.IsMoving = false;

        }

        _characterController.PushIfDead();
        _characterController.HandleStopMovement();

    }

    public void AddEffects(List<IEffect> effects)
    {
        if (effects != null && effects.Count > 0)
        {
            foreach (IEffect effect in effects)
            {
                IEffect effectCopy = effect.Clone();
                _characterController.Effector.AddEffect(effectCopy);
            }
        }
    }

    public void ViewRotation()
    {
    }

    public void OnExit()
    {
        //Debug.Log("<color=#FFFFFF>" + "--|Exit Dead Condition State|-- " + "</color>");
    }

    public void UseSlingshotAsync(PointerEventData eventData, Transform slingShotInitPosition)
    {
    }

    public void LaunchYourselfToPoint(Vector3 point)
    {
    }

    public void Attack()
    {
    }

    public void Move()
    {
    }

    public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer)
    {
    }
}