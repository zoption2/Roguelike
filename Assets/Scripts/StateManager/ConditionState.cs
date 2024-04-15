using CharactersStats;
using Interactions;
using SlingShotLogic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IConditionState
{
    public void OnEnter();
    public void OnExit();
    public IInteraction GetInteraction(InteractionType interactionType);
    public void ApplyInteraction(IInteraction interaction);
    public void Launch(Vector2 direction);
    public void AddEffects(List<IEffect> effects);
    public void ViewRotation();
    public void DoUpdate();
    public void UseSlingshot(PointerEventData eventData, Transform slingShotInitPosition);
    public void LaunchToPoint(Vector3 point);
}

public class ActiveState
{
    protected ICharacterController _characterController;
    protected ISlingShot _slingShot;

    public ActiveState(ICharacterController characterController)
    {
        _characterController = characterController;
    }
    public void OnEnter()
    {
        Debug.Log("<color=#44F44F>" + "--|Enter Active State|-- " + "</color>");

    }
    public void DoUpdate()
    {
        if (_characterController.CharacterView.Rigidbody.velocity.magnitude > _characterController.CharacterView.MaxVelocity)
        {
            _characterController.CharacterView.Rigidbody.velocity = _characterController.CharacterView.Rigidbody.velocity.normalized * _characterController.CharacterView.MaxVelocity;
        }

        if (_characterController.CharacterView.Rigidbody.velocity.magnitude > 0.5f && !_characterController.IsMoving)
        {
            _characterController.IsMoving = true;
        }
        else if (_characterController.CharacterView.Rigidbody.velocity.magnitude < 0.2f && _characterController.CharacterView.Rigidbody.velocity.magnitude > 0f && _characterController.IsMoving)
        {
            _characterController.IsMoving = false;
            _characterController.HandleStopMovement();
        }

        if (_characterController.IsMoving)
        {
            ViewRotation();
        }
    }
    public virtual void Launch(Vector2 direction)
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
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        _characterController.CharacterView.Rigidbody.rotation = Quaternion.Slerp(_characterController.CharacterView.Rigidbody.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    public void OnExit()
    {
        //Debug.Log("<color=#44F44F>" + "--|Exit Active State|-- " + "</color>");
        if(_slingShot != null)
        {
            _slingShot.OnDirectionChange -= _characterController.CharacterView.ChangeDirection;
            _slingShot.OnShoot -= Launch;
        }
        
    }

    public void ApplyInteraction(IInteraction interaction)
    {
    }

    public virtual void UseSlingshot(PointerEventData eventData, Transform slingShotInitPosition)
    {
    }
}

public class PlayerActiveState : ActiveState, IConditionState
{
    
    public PlayerActiveState(ICharacterController characterController) : base(characterController)
    {
    }

    public override void UseSlingshot(PointerEventData eventData, Transform slingShotInitPosition)
    {
        CharacterType type = _characterController.GetCharacterType();

        Vector3 fixedInitPosition = new Vector3(slingShotInitPosition.position.x, slingShotInitPosition.position.y, slingShotInitPosition.position.z - 1f);

        _slingShot = _characterController.SlingShotPooler.Pull<ISlingShot>(type, fixedInitPosition, Quaternion.identity, slingShotInitPosition.parent);

        _slingShot.Init(slingShotInitPosition.position, type);

        _slingShot.OnDirectionChange -= _characterController.CharacterView.ChangeDirection;
        _slingShot.OnDirectionChange += _characterController.CharacterView.ChangeDirection;

        _slingShot.OnShoot -= Launch;
        _slingShot.OnShoot += Launch;

        DragInputModule.dragFocusObject = _slingShot.gameObject;
        eventData.pointerDrag = _slingShot.gameObject;
        eventData.dragging = true;
    }

    public override void Launch(Vector2 direction)
    {
        base.Launch(direction); 

        _slingShot.OnShoot -= Launch;
    }

    public void LaunchToPoint(Vector3 point)
    {
    }
}

public class EnemyActiveState : ActiveState, IConditionState
{
    public EnemyActiveState(ICharacterController characterController) : base(characterController)
    {
    }

    public void LaunchToPoint(Vector3 point)
    {
        float launchPower = _characterController.ModifiableStats.LaunchPower.Value;
        Vector3 direction = point - _characterController.GetTransform().position;
        float distance = direction.magnitude;
        direction.Normalize();
        float multiplier = Mathf.Clamp(distance, 4, launchPower);
        Vector3 initialVelocity = direction * multiplier;
        _characterController.CharacterView.Rigidbody.AddForce(initialVelocity, ForceMode.VelocityChange);
    }
}

public class InactiveState : IConditionState
{
    private ICharacterController _characterController;

    public InactiveState(ICharacterController characterController)
    {
        _characterController = characterController;
    }
    public void DoUpdate()
    {
        if (_characterController.CharacterView.Rigidbody.velocity.magnitude > _characterController.CharacterView.MaxVelocity)
        {
            _characterController.CharacterView.Rigidbody.velocity = _characterController.CharacterView.Rigidbody.velocity.normalized * _characterController.CharacterView.MaxVelocity;
        }

        if (_characterController.CharacterView.Rigidbody.velocity.magnitude > 0.5f && !_characterController.IsMoving)
        {
            _characterController.IsMoving = true;
        }
        else if (_characterController.CharacterView.Rigidbody.velocity.magnitude < 0.2f && _characterController.CharacterView.Rigidbody.velocity.magnitude > 0f && _characterController.IsMoving)
        {
            _characterController.IsMoving = false;
            _characterController.HandleStopMovement();
        }

        if (_characterController.IsMoving)
        {
            ViewRotation();
        }
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
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle + 90f);
        _characterController.CharacterView.Rigidbody.rotation = Quaternion.Slerp(_characterController.CharacterView.Rigidbody.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void OnExit()
    {
        //Debug.Log("<color=#C0C8D8>" + "--|Exit InactiveState State|-- " + "</color>");
    }

    public void Launch(Vector2 direction)
    {
    }

    public void UseSlingshot(PointerEventData eventData, Transform slingShotInitPosition)
    {
    }

    public void LaunchToPoint(Vector3 point)
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
    public void DoUpdate()
    {
        
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

    public void UseSlingshot(PointerEventData eventData, Transform slingShotInitPosition)
    {
    }

    public void LaunchToPoint(Vector3 point)
    {
    }
}

public class StunState : IConditionState
{
    private ICharacterController _characterController;

    public StunState(ICharacterController characterController)
    {
        _characterController = characterController;
    }
    public void DoUpdate()
    {
        if (_characterController.CharacterView.Rigidbody.velocity.magnitude > _characterController.CharacterView.MaxVelocity)
        {
            _characterController.CharacterView.Rigidbody.velocity = _characterController.CharacterView.Rigidbody.velocity.normalized * _characterController.CharacterView.MaxVelocity;
        }


        if (_characterController.CharacterView.Rigidbody.velocity.magnitude > 0.5f && !_characterController.IsMoving)
        {
            _characterController.IsMoving = true;
        }
        else if (_characterController.CharacterView.Rigidbody.velocity.magnitude < 0.2f && _characterController.CharacterView.Rigidbody.velocity.magnitude > 0f && _characterController.IsMoving)
        {
            _characterController.IsMoving = false;
            _characterController.HandleStopMovement();
        }

        if (_characterController.IsMoving)
        {
            ViewRotation();
        }
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
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        _characterController.CharacterView.Rigidbody.rotation = Quaternion.Slerp(_characterController.CharacterView.Rigidbody.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    public void OnExit()
    {
        _characterController.IsStunned = false;
        //Debug.Log("<color=#FFFFF>" + "--|Exit Stun Condition State|-- " + "</color>");
    }

    public void UseSlingshot(PointerEventData eventData, Transform slingShotInitPosition)
    {
    }

    public void LaunchToPoint(Vector3 point)
    {
    }
}

