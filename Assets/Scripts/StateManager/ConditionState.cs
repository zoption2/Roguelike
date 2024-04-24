using BehaviourTree;
using CharactersStats;
using Interactions;
using SlingShotLogic;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public interface IConditionState
{
    public void OnEnter();
    public void OnExit();
    public IInteraction GetInteraction(InteractionType interactionType);
    public void ApplyInteraction(IInteraction interaction);
    public void LaunchYourself(Vector2 direction);
    public void AddEffects(List<IEffect> effects);
    public void ViewRotation();
    public void DoUpdate();
    public void UseSlingshot(PointerEventData eventData, Transform slingShotInitPosition);
    public void LaunchYourselfToPoint(Vector3 point);
    public void Attack();
    public void Move();
    public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer);
}

public abstract class ActiveState
{
    protected ICharacterController _characterController;
    protected ISlingShot _slingShot;
    protected int _milisecondsDelay = 3000;

    public ActiveState(ICharacterController characterController)
    {
        _characterController = characterController;
    }

    public void OnEnter()
    {
        Debug.Log("<color=#44F44F>" + "--|Enter Active State|-- " + "</color>");
        //_characterController.ActivateUI();
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
        else if (_characterController.GetVelocity().magnitude < 0.1f && _characterController.GetVelocity().magnitude > 0f && _characterController.IsMoving)
        {
            _characterController.IsMoving = false;
            _characterController.HandleStopMovement();
        }

        if (_characterController.IsMoving)
        {
            ViewRotation();
        }
    }

    public virtual void LaunchYourself(Vector2 direction)
    {
        //float dragConstant = _characterController.CharacterView.Rigidbody.drag;
        float launchPower = _characterController.ModifiableStats.LaunchPower.Value;
        direction.Normalize();
        Vector2 forceVector = direction * launchPower;
        _characterController.GetRigidbody().AddForce(forceVector, ForceMode.VelocityChange);
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
        Vector3 velocity = _characterController.GetVelocity();
        float rotationSpeed = velocity.magnitude;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        _characterController.GetRigidbody().rotation = Quaternion.Slerp(_characterController.GetRigidbody().rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void OnExit()
    {
        //Debug.Log("<color=#44F44F>" + "--|Exit Active State|-- " + "</color>");
        if(_slingShot != null)
        {
            _slingShot.OnDirectionChange -= _characterController.CharacterView.ChangeDirection;
            _slingShot.OnShoot -= LaunchYourself;
        }
    }

    public void ApplyInteraction(IInteraction interaction)
    {
    }

    public virtual void UseSlingshot(PointerEventData eventData, Transform slingShotInitPosition)
    {
    }

    public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer)
    {
        bumpFromDealer.ApplyForce(_characterController.CharacterView, interactible);
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

        _slingShot.OnShoot -= LaunchYourself;
        _slingShot.OnShoot += LaunchYourself;

        DragInputModule.dragFocusObject = _slingShot.gameObject;
        eventData.pointerDrag = _slingShot.gameObject;
        eventData.dragging = true;
    }

    public override void LaunchYourself(Vector2 direction)
    {
        //base.Launch(direction);

        float launchPower = _characterController.ModifiableStats.LaunchPower.Value;
        Vector2 forceVector = direction * launchPower;
        _characterController.GetRigidbody().AddForce(forceVector, ForceMode.VelocityChange);

        _slingShot.OnShoot -= LaunchYourself;
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

public class EnemyActiveState : ActiveState, IConditionState
{
    public EnemyActiveState(ICharacterController characterController) : base(characterController)
    {
    }

    public void LaunchYourselfToPoint(Vector3 point)
    {
        float minLaunchPower = 1f;
        float maxLaunchPower = _characterController.ModifiableStats.LaunchPower.Value;
        Vector2 direction = point - _characterController.GetTransform().position;
        float distance = Vector2.Distance(point, _characterController.GetTransform().position);
        direction.Normalize();
        float dragConstant = _characterController.GetRigidbody().drag;
        
        float multiplier = Mathf.Clamp(distance * dragConstant, minLaunchPower, maxLaunchPower);
        //Debug.Log("multiplier: " + multiplier * dragConstant);
        Vector2 initialVelocity = direction * multiplier; //* dragConstant;
        //Debug.Log("InitialVelocityMove: " + initialVelocity.magnitude);
        _characterController.GetRigidbody().AddForce(initialVelocity, ForceMode.VelocityChange);
    }

    public void Attack()
    {
        Transform target = _characterController.DefaultBehaviourTree.GetTarget();
        Transform enemy = _characterController.GetTransform();
        Vector2 direction = enemy.position - target.position;
        _characterController.CharacterView.ChangeDirection(-direction);
        LaunchYourself(direction * -1);
    }

    public async void Move()
    {
        NavMeshObstacle navObstacle = _characterController.NavMeshObstacle;
        IDefaultBehaviourTree defaultBehaviourTree = _characterController.DefaultBehaviourTree;
        navObstacle.enabled = false;
        await Task.Delay(_milisecondsDelay / 10);

        Transform target = defaultBehaviourTree.GetTarget();

        Transform enemy = _characterController.GetTransform();

        _characterController.NavMeshAgent.enabled = true;
        _characterController.NavMeshAgent.SetDestination(target.position);
        await Task.Delay(_milisecondsDelay / 10);

        NavMeshPath path = _characterController.NavMeshAgent.path;

        Vector3 waypoint = defaultBehaviourTree.FindWaypointToObserveTarget(path, target);
        _characterController.NavMeshAgent.enabled = false;
        Vector2 direction = enemy.position - waypoint;
        _characterController.CharacterView.ChangeDirection(-direction);

        LaunchYourselfToPoint(waypoint);
        navObstacle.enabled = true;
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
        if (_characterController.GetVelocity().magnitude > _characterController.CharacterView.MaxVelocity)
        {
            _characterController.GetRigidbody().velocity = _characterController.GetVelocity().normalized * _characterController.CharacterView.MaxVelocity;
        }

        if (_characterController.GetVelocity().magnitude > 0.5f && !_characterController.IsMoving)
        {
            _characterController.IsMoving = true;
        }
        else if (_characterController.GetVelocity().magnitude < 0.1f && _characterController.GetVelocity().magnitude > 0f && _characterController.IsMoving)
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
        _characterController.ModifiableStats = _characterController.InteractionCalculator.CalculateInteractionResult(_characterController.ModifiableStats, interactionResult);
        _characterController.AnalizeCondition();
    }

    public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer)
    {
        IMovable bump = new Bounce();
        bump.ApplyForce(_characterController.CharacterView, interactible);
    }

    public void OnEnter()
    {
        Debug.Log("<color=#C0C8D8>" + "--|Enter InactiveState State|-- " + "</color>");
        //_characterController.DisableUI();
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
        Vector3 velocity = _characterController.GetVelocity();
        float rotationSpeed = velocity.magnitude;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        _characterController.GetRigidbody().rotation = Quaternion.Slerp(_characterController.GetRigidbody().rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void OnExit()
    {
        //Debug.Log("<color=#C0C8D8>" + "--|Exit InactiveState State|-- " + "</color>");
    }

    public void LaunchYourself(Vector2 direction)
    {
    }

    public void UseSlingshot(PointerEventData eventData, Transform slingShotInitPosition)
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

    public void LaunchYourself(Vector2 direction)
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

public class StunState : IConditionState
{
    private ICharacterController _characterController;

    public StunState(ICharacterController characterController)
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
        else if (_characterController.GetVelocity().magnitude < 0.2f && _characterController.GetVelocity().magnitude > 0f && _characterController.IsMoving)
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

    public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer)
    {
        IMovable bump = new Bounce();
        bump.ApplyForce(_characterController.CharacterView, interactible);
    }

    public void LaunchYourself(Vector2 direction)
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
        Vector3 velocity = _characterController.GetVelocity();
        float rotationSpeed = velocity.magnitude;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle + 90f);
        _characterController.GetRigidbody().rotation = Quaternion.Slerp(_characterController.GetRigidbody().rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void OnExit()
    {
        _characterController.IsStunned = false;
        //Debug.Log("<color=#FFFFF>" + "--|Exit Stun Condition State|-- " + "</color>");
    }

    public void UseSlingshot(PointerEventData eventData, Transform slingShotInitPosition)
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

