using Abilities;
using BehaviourTree;
using Pool;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyActiveState : ActiveState, IConditionState
{
    public EnemyActiveState(ICharacterController characterController, ProjectilePooler projectilePooler) : base(characterController, projectilePooler)
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

        if (currentAbility.ProjectileType == ProjectileType.None)
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