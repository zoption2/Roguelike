using CharactersStats;
using Interactions;
using System.Collections.Generic;
using UnityEngine;

public interface IConditionState
{
    public void OnEnter();
    public void OnExit();
    public IInteraction GetInteraction(InteractionType interactionType);
    public void ApplyInteraction(IInteraction interaction);
    public void Launch(Vector2 direction);
    public void AddEffects(List<IEffect> effects);
    public void ViewRotation();
}

public class ActiveState : IConditionState
{
    private ICharacterController _characterController;

    public ActiveState(ICharacterController characterController)
    {
        _characterController = characterController;
    }
    public void OnEnter()
    {
        Debug.Log("<color=#44F44F>" + "--|Enter Active State|-- " + "</color>");

    }
    public void Launch(Vector2 direction)
    {
        float launchPower = _characterController.ModifiableStats.LaunchPower.Value;
        direction.Normalize();
        Vector2 forceVector = direction * launchPower;
        _characterController.CharacterView.Rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
    }
    public IInteraction GetInteraction(InteractionType interactionType)
    {
        ReactiveStats statsWithBonus = _characterController.Effector.ProcessStatsBeforeInteraction(_characterController.ModifiableStats);
        _characterController.InteractionDealer.Init(statsWithBonus);
        IInteraction interaction = _characterController.InteractionDealer.UseInteraction(interactionType);
        return interaction;
    }

    public void AddEffects(List<IEffect> effects)
    {
        if (effects != null && effects.Count > 0)
        {
            foreach (IEffect effect in effects)
            {
                _characterController.Effector.AddEffects(effects);
            }
        }
    }

    public void ViewRotation()
    {
        Vector3 velocity = _characterController.CharacterView.Rigidbody.velocity;
        float rotationSpeed = velocity.magnitude;
        float angle = Mathf.Atan2(velocity.y, velocity.x);
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

        _characterController.CharacterView.Rigidbody.rotation = Quaternion.Slerp(_characterController.CharacterView.Rigidbody.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    public void OnExit()
    {
        //Debug.Log("<color=#44F44F>" + "--|Exit Active State|-- " + "</color>");
    }

    public void ApplyInteraction(IInteraction interaction)
    {
    }
}

public class InactiveState : IConditionState
{
    private ICharacterController _characterController;

    public InactiveState(ICharacterController characterController)
    {
        _characterController = characterController;
    }

    public IInteraction GetInteraction(InteractionType interactionType)
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
                _characterController.Effector.AddEffects(effects);
            }
        }
        ReactiveStats interactionResult = _characterController.InteractionProcessor.ProcessInteraction(interaction);

        _characterController.ModifiableStats = _characterController.InteractionFinalizer.CalculateInteractionResult(_characterController.ModifiableStats, interactionResult);

        _characterController.AnalizeCondition();

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
                _characterController.Effector.AddEffects(effects);
            }
        }
    }

    public void ViewRotation()
    {
        Vector3 velocity = _characterController.CharacterView.Rigidbody.velocity;
        float rotationSpeed = velocity.magnitude;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion targetRotation;

        targetRotation = Quaternion.Euler(0f, 0f, angle + 90f);

        _characterController.CharacterView.Rigidbody.rotation = Quaternion.Slerp(_characterController.CharacterView.Rigidbody.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void OnExit()
    {
        //Debug.Log("<color=#C0C8D8>" + "--|Exit InactiveState State|-- " + "</color>");
    }

    public void Launch(Vector2 direction)
    {
    }
}

public class DeadState : IConditionState
{
    private ICharacterController _characterController;

    public DeadState(ICharacterController characterController)
    {
        _characterController = characterController;
    }

    public void ApplyInteraction(IInteraction interaction)
    {
    }

    public IInteraction GetInteraction(InteractionType interactionType)
    {
        IInteraction interaction = _characterController.InteractionDealer.UseInteraction(InteractionType.None);
        return interaction;
    }

    public void Launch(Vector2 direction)
    {
    }

    public void OnEnter()
    {
        Debug.Log("<color=#FFFFFF>" + "--|Enter Dead Condition State|-- " + "</color>");
        _characterController.PushIfDead();
    }

    public void AddEffects(List<IEffect> effects)
    {
        if (effects != null && effects.Count > 0)
        {
            foreach (IEffect effect in effects)
            {
                _characterController.Effector.AddEffects(effects);
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
}

public class StunState : IConditionState
{
    private ICharacterController _characterController;

    public StunState(ICharacterController characterController)
    {
        _characterController = characterController;
    }

    public void ApplyInteraction(IInteraction interaction)
    {
    }

    public IInteraction GetInteraction(InteractionType interactionType)
    {
        IInteraction interaction = _characterController.InteractionDealer.UseInteraction(InteractionType.None);
        return interaction;
    }

    public void Launch(Vector2 direction)
    {
    }

    public void OnEnter()
    {
        _characterController.IsStunned = true;
        Debug.Log("<color=#FFFFFF>" + "--|Enter Stun Condition State|-- " + "</color>");
        
    }

    public void AddEffects(List<IEffect> effects)
    {
        if (effects != null && effects.Count > 0)
        {
            foreach (IEffect effect in effects)
            {
                _characterController.Effector.AddEffects(effects);
            }
        }
    }
    public void ViewRotation()
    {
        Vector3 velocity = _characterController.CharacterView.Rigidbody.velocity;
        float rotationSpeed = velocity.magnitude;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion targetRotation;

        targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);


        _characterController.CharacterView.Rigidbody.rotation = Quaternion.Slerp(_characterController.CharacterView.Rigidbody.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    public void OnExit()
    {
        _characterController.IsStunned = false;
        //Debug.Log("<color=#FFFFF>" + "--|Exit Stun Condition State|-- " + "</color>");
    }
}

