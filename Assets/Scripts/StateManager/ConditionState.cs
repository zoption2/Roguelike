using BehaviourTree;
using CharactersStats;
using Interactions;
using SlingShotLogic;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Abilities;
using Zenject;
using Pool;
using Projectiles;


public interface IConditionState
{
    public void OnEnter();
    public void OnExit();
    public IInteraction GetInteraction();
    public void ApplyInteraction(IInteraction interaction);
    public void LaunchYourself(Vector3 direction);
    public void AddEffects(List<IEffect> effects);
    public void ViewRotation();
    public void DoUpdate();
    public void UseSlingshot(PointerEventData eventData, Transform slingShotInitPosition);
    public void LaunchYourselfToPoint(Vector3 point);
    public void Attack();
    public void Move();
    public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer);
}

public delegate void OnStopped();
public abstract class ActiveState
{
    protected ICharacterController _characterController;
    protected ISlingShot _slingShot;
    protected int _milisecondsDelay = 3000;
    protected event OnStopped ON_STOPPED;
    protected NavMeshAgent _navAgent;
    protected NavMeshObstacle _navObstacle;
    protected ProjectilePooler _projectilePooler;

    public ActiveState(ICharacterController characterController,ProjectilePooler projectilePooler)
    {
        _characterController = characterController;
        _navAgent = _characterController.NavMeshAgent;
        _navObstacle = _characterController.NavMeshObstacle;
        _projectilePooler = projectilePooler;
    }

    public void OnEnter()
    {
        Debug.Log("<color=#44F44F>" + "--|Enter Active State|-- " + "</color>");
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
        else if (_navAgent.enabled && _navAgent.velocity.magnitude != 0)
        {
            AdjustRotationForNavAgent();
        }

        if (_navAgent.enabled && _navAgent.velocity.magnitude == 0 && ON_STOPPED != null)
        {
            _navAgent.enabled = false;
            _navObstacle.enabled = true;
            ON_STOPPED?.Invoke();
        }
    }


    protected Vector3 GetForceVector(Vector3 direction)
    {
        float launchPower = _characterController.ModifiableStats.LaunchPower.Value;
        direction.Normalize();
        IAbility currentAbility = _characterController.CurrentAbility;
        float launchMultiplier = currentAbility.GetLaunchModifier();
        Vector3 forceVector = direction * launchPower * launchMultiplier;

        return forceVector;
    }

    public virtual void LaunchYourself(Vector3 direction)
    {
        Vector3 forceVector = GetForceVector(direction);
        _characterController.GetRigidbody().AddForce(forceVector, ForceMode.VelocityChange);
    }

    public virtual async void LaunchProjectile(Vector3 direction)
    {
        Vector3 forceVector = GetForceVector(direction);
        IAbility currentAbility = _characterController.CurrentAbility;
        Transform transform = _characterController.GetTransform();
        Transform spawn = _characterController.GetProjectileSpawn();

        IMyPoolable projectilePoolable = await _projectilePooler.Pull<IMyPoolable>(currentAbility.ProjectileType, spawn.position, transform.rotation, transform.parent);
        Projectile projectile = projectilePoolable.gameObject.GetComponent<Projectile>();

        if(projectile.ControllerInputs == null)
            projectile.Init((IControllerInputs)_characterController, currentAbility.ProjectileType,_projectilePooler);

        projectile.SetRicochetCount(currentAbility.RicochetCount);
        projectile.GetRigidbody().velocity = forceVector;
    }

    public IInteraction GetInteraction()
    {
        ReactiveStats statsWithBonus = _characterController.Effector.ProcessStatsBeforeInteraction(_characterController.ModifiableStats);
        IInteraction result = null;

        if (_characterController.CurrentAbility != null)
        {
            IInteraction interaction = _characterController.CurrentAbility.Interaction;
            interaction.SetStats(statsWithBonus);
            result = interaction;
        }

        return result;
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
        Transform character = _characterController.GetTransform();
        Vector3 velocity = _characterController.GetVelocity();
        float rotationSpeed = velocity.magnitude;
        float angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);
        character.rotation = Quaternion.Slerp(character.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void AdjustRotationForNavAgent()
    {
        Transform character = _characterController.GetTransform();
        float rotationSpeed = 2f;

        Vector3 direction = (_navAgent.steeringTarget - character.position).normalized;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);

        character.rotation = Quaternion.Slerp(character.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void OnExit()
    {
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
    public PlayerActiveState(ICharacterController characterController, ProjectilePooler projectilePooler)
        : base(characterController, projectilePooler)
    {
    }

    public override async void UseSlingshot(PointerEventData eventData, Transform slingShotInitPosition)
    {
        CharacterType type = _characterController.GetCharacterType();

        Vector3 fixedInitPosition = new Vector3(slingShotInitPosition.position.x, slingShotInitPosition.position.y, slingShotInitPosition.position.z);

        _slingShot = await _characterController.SlingShotPooler.Pull<ISlingShot>(type, fixedInitPosition, Quaternion.Euler(90, 0, 0), slingShotInitPosition.parent);

        _slingShot.Init(slingShotInitPosition.position, type,_characterController.GetCurrentLaunchDistance());

        _slingShot.OnDirectionChange -= _characterController.CharacterView.ChangeDirection;
        _slingShot.OnDirectionChange += _characterController.CharacterView.ChangeDirection;
        IAbility currentAbility = _characterController.CurrentAbility;
        if(currentAbility.ProjectileType == ProjectileType.None)
        {
            _slingShot.OnShoot -= LaunchProjectile;
            _slingShot.OnShoot -= LaunchYourself;
            _slingShot.OnShoot += LaunchYourself;
        }
        else
        {
            _slingShot.OnShoot -= LaunchYourself;
            _slingShot.OnShoot -= LaunchProjectile;
            _slingShot.OnShoot += LaunchProjectile;
        }

        _slingShot.OnAbilityUse -= _characterController.ProcessReloadAbility;
        _slingShot.OnAbilityUse += _characterController.ProcessReloadAbility;

        DragInputModule.dragFocusObject = _slingShot.gameObject;
        eventData.pointerDrag = _slingShot.gameObject;
        eventData.dragging = true;
    }

    public override void LaunchYourself(Vector3 direction)
    {
        float slingshotRadius = 2.1f;
        float slingshotMultiplier = direction.magnitude / slingshotRadius;
        Vector3 forceVector = GetForceVector(direction);
        _characterController.GetRigidbody().velocity = forceVector * slingshotMultiplier;

        _slingShot.OnShoot -= LaunchYourself;

    }

    public override void LaunchProjectile(Vector3 direction)
    {
        base.LaunchProjectile(direction);
        _slingShot.OnShoot -= LaunchProjectile;
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
    public EnemyActiveState(ICharacterController characterController, ProjectilePooler projectilePooler)
        : base(characterController, projectilePooler)
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
        Vector2 initialVelocity = direction * multiplier;
        _characterController.GetRigidbody().AddForce(initialVelocity, ForceMode.VelocityChange);
    }

    public void Attack()
    {
        Transform target = _characterController.DefaultBehaviourTree.GetTarget();
        Transform enemy = _characterController.GetTransform();
        Vector3 direction = target.position - enemy.position;
        _characterController.CharacterView.ChangeDirection(direction);
        IAbility currentAbility = _characterController.CurrentAbility;
        
        if(currentAbility.ProjectileType == ProjectileType.None)
        {
            LaunchYourself(direction);
        }
        else
        {
            LaunchProjectile(direction);
        }
        currentAbility.UseAbility();
    }

    public async void Move()
    {
        IDefaultBehaviourTree defaultBehaviourTree = _characterController.DefaultBehaviourTree;
        Transform target = defaultBehaviourTree.GetTarget();
        _navObstacle.enabled = false;
        
        await Task.Delay(_milisecondsDelay / 10);
        _navAgent.enabled = true;        

        _navAgent.SetDestination(target.position);
        await Task.Delay(_milisecondsDelay / 10);
        if (defaultBehaviourTree.CanAttackAfterMove(_navAgent.path))
        {
            ON_STOPPED += Attack;
        }
        else
        {
            ON_STOPPED += _characterController.HandleStopMovement;
        }
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
        else if (_characterController.GetVelocity().magnitude < 0.05f && _characterController.GetVelocity().magnitude > 0f && _characterController.IsMoving)
        {
            _characterController.IsMoving = false;
            _characterController.HandleStopMovement();
        }
        else if(_characterController.GetVelocity().magnitude == 0 && _characterController.IsMoving)
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
        _characterController.PushIfDead();
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
        else if (_characterController.GetVelocity().magnitude < 0.05f && _characterController.GetVelocity().magnitude > 0f && _characterController.IsMoving)
        {
            _characterController.IsMoving = false;
            _characterController.HandleStopMovement();
        }
        else if(_characterController.GetVelocity().magnitude == 0 && _characterController.IsMoving)
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

    public IInteraction GetInteraction()
    {
        IInteraction interaction = _characterController.InteractionDealer.UseInteraction(InteractionType.None);
        return interaction;
    }

    public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer)
    {
        bumpFromDealer.ApplyForce(interactible, _characterController.CharacterView);
    }

    public void LaunchYourself(Vector3 direction)
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
        _characterController.IsStunned = false;
        Debug.Log("<color=#FFFFF>" + "--|Exit Stun Condition State|-- " + "</color>");
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

