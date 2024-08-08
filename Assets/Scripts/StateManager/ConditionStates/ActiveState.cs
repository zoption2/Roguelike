using Abilities;
using CharactersStats;
using Interactions;
using Pool;
using Projectiles;
using SlingShotLogic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public abstract class ActiveState
{
    protected ICharacterController _characterController;
    protected ISlingShot _slingShot;
    protected int _milisecondsDelay = 3000;
    protected event OnStopped ON_STOPPED;
    protected NavMeshAgent _navAgent;
    protected NavMeshObstacle _navObstacle;
    protected ProjectilePooler _projectilePooler;

    public ActiveState(ICharacterController characterController, ProjectilePooler projectilePooler)
    {
        _characterController = characterController;
        _navAgent = _characterController.NavMeshAgent;
        _navObstacle = _characterController.NavMeshObstacle;
        _projectilePooler = projectilePooler;
    }

    public virtual void OnEnter()
    {
        Debug.Log("<color=#44F44F>" + "--|Enter Active State|-- " + "</color>");

        //_characterController.SwitchState(TypeOfConditionState.Preparation);
    }

    public void DoUpdate()
    {
        if (_characterController.GetVelocity().magnitude > _characterController.CharacterView.MaxVelocity)
        {
            _characterController.GetRigidbody().velocity = _characterController.GetVelocity().normalized * _characterController.CharacterView.MaxVelocity;
        }

        if (_characterController.GetVelocity().magnitude > 0.5f && !_characterController.IsMoving)
        {
            _characterController.MoveAnimation.Play(() => { });
            _characterController.IsMoving = true;
        }
        else if (_characterController.GetVelocity().magnitude < 0.05f && _characterController.GetVelocity().magnitude > 0f && _characterController.IsMoving)
        {
            _characterController.MoveAnimation.Stop(() => { });
            _characterController.IsMoving = false;
            _characterController.HandleStopMovement();
        }
        else if (_characterController.GetVelocity().magnitude == 0 && _characterController.IsMoving)
        {
            _characterController.MoveAnimation.Stop(() => { });
            _characterController.IsMoving = false;
            _characterController.HandleStopMovement();
        }

        if (_characterController.IsMoving)
        {
            ViewRotation();
        }
        else if (_navAgent != null && _navAgent.enabled && _navAgent.velocity.magnitude != 0)
        {
            AdjustRotationForNavAgent();
        }

        if (_navAgent != null && _navAgent.enabled && _navAgent.velocity.magnitude == 0 && ON_STOPPED != null)
        {
            _navAgent.enabled = false;
            if (_navObstacle != null)
            {
                _navObstacle.enabled = true;
            }
            ON_STOPPED?.Invoke();
            ON_STOPPED = null;
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

    public virtual void LaunchProjectile(Vector3 direction)
    {
        Vector3 forceVector = GetForceVector(direction);
        IAbility currentAbility = _characterController.CurrentAbility;
        Transform transform = _characterController.GetTransform();
        Transform spawn = _characterController.GetProjectileSpawn();

        IMyPoolable projectilePoolable = _projectilePooler.Pull<IMyPoolable>(currentAbility.ProjectileType, spawn.position, transform.rotation, transform.parent);
        IProjectile projectile = projectilePoolable.gameObject.GetComponent<IProjectile>();

        if (projectile.ControllerInputs == null)
            projectile.Init((IControllerInputs)_characterController, currentAbility.ProjectileType, _projectilePooler);

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

    public virtual void OnExit()
    {
        if (_slingShot != null)
        {
            _slingShot.OnDirectionChange -= _characterController.CharacterView.ChangeDirection;
            _slingShot.OnShoot -= LaunchYourself;
        }
    }

    public void ApplyInteraction(IInteraction interaction)
    {
    }

    public virtual void UseSlingshotAsync(PointerEventData eventData, Transform slingShotInitPosition)
    {
    }

    public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer)
    {
        bumpFromDealer.ApplyForce(_characterController.CharacterView, interactible);
    }
}