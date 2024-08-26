using CharactersStats;
using Interactions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InactiveState : IConditionState
{
    private ICharacterController _characterController;

    public InactiveState(ICharacterController characterController)
    {
        _characterController = characterController;
    }

    public void DoUpdate()
    {
        if (_characterController.GetVelocity().magnitude > _characterController.CharacterView.MaxVelocity)
        {
            _characterController.GetRigidbody().velocity = _characterController.GetVelocity().normalized * _characterController.CharacterView.MaxVelocity;
        }


        if (_characterController.GetVelocity().magnitude > 0.5f && !_characterController.IsMoving)
        {
            _characterController.IsMoving = true;
        }
        else if (_characterController.GetVelocity().magnitude < 0.05f && _characterController.GetVelocity().magnitude > 0f && _characterController.IsMoving)
        {
            _characterController.IsMoving = false;
            _characterController.HandleStopMovement();
        }
        else if (_characterController.GetVelocity().magnitude == 0 && _characterController.IsMoving)
        {
            _characterController.IsMoving = false;
            _characterController.HandleStopMovement();
        }

        if (_characterController.IsMoving)
        {
            ViewRotation();
        }
    }

    public IInteraction GetInteraction()
    {
        IInteraction interaction = _characterController.InteractionDealer.UseInteraction(InteractionType.None);
        return interaction;
    }

    public void ApplyInteraction(IInteraction interaction)
    {
        List<IEffect> effects = interaction.GetEffects();
        if (effects != null && effects.Count > 0)
        {
            foreach (IEffect effect in effects)
            {
                IEffect effectCopy = effect.Clone();
                _characterController.Effector.AddEffect(effectCopy);
            }
        }

        ReactiveStats interactionResult = _characterController.InteractionProcessor.ProcessInteraction(interaction);
        _characterController.ModifiableStats = _characterController.InteractionCalculator.CalculateInteractionResult(_characterController.ModifiableStats, interactionResult);
        _characterController.AnalizeCondition();
    }

    public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer)
    {
        bumpFromDealer.ApplyForce(_characterController.CharacterView, interactible);
    }

    public void OnEnter()
    {
        Debug.Log("<color=#C0C8D8>" + "--|Enter InactiveState State|-- " + "</color>");
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
        Vector3 velocity = _characterController.GetVelocity();
        float rotationSpeed = velocity.magnitude;
        float angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);
        _characterController.GetRigidbody().rotation = Quaternion.Slerp(_characterController.GetRigidbody().rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void OnExit()
    {
        //Debug.Log("<color=#C0C8D8>" + "--|Exit InactiveState State|-- " + "</color>");
    }

    public void LaunchYourself(Vector3 direction)
    {
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
}