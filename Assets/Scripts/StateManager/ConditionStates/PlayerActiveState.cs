using Abilities;
using Gameplay;
using Pool;
using SlingShotLogic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerActiveState : ActiveState, IConditionState
{
    public PlayerActiveState(ICharacterController characterController, ProjectilePooler projectilePooler) : base(characterController, projectilePooler)
    {
    }

    public override void UseSlingshotAsync(PointerEventData eventData, Transform slingShotInitPosition)
    {
        CharacterType type = _characterController.GetCharacterType();

        Vector3 fixedInitPosition = new Vector3(slingShotInitPosition.position.x, slingShotInitPosition.position.y, slingShotInitPosition.position.z);

        _slingShot = _characterController.SlingShotPooler.Pull<ISlingShot>(type, fixedInitPosition, Quaternion.Euler(90, 0, 0), slingShotInitPosition.parent);

        _slingShot.Init(slingShotInitPosition.position, type, _characterController.GetCurrentLaunchDistance());

        _slingShot.OnDirectionChange -= _characterController.CharacterView.ChangeDirection;
        _slingShot.OnDirectionChange += _characterController.CharacterView.ChangeDirection;
        IAbility currentAbility = _characterController.CurrentAbility;
        if (currentAbility.ProjectileType == ProjectileType.None)
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