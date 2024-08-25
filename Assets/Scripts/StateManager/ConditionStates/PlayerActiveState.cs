using Abilities;
using Player;
using Pool;
using SlingShotLogic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerActiveState : ActiveState, IConditionState
{
    IPlayerController _playerController;
    public PlayerActiveState(ICharacterController characterController, ProjectilePooler projectilePooler) : base(characterController, projectilePooler)
    {
        _playerController = (IPlayerController)characterController;
    }

    public override void UseSlingshotAsync(PointerEventData eventData, Transform slingShotInitPosition)
    {
        _playerController.PlayerAttackWaitingAnimation.Play(() => { });

        CharacterType type = _characterController.GetCharacterType();

        Vector3 fixedInitPosition = new Vector3(slingShotInitPosition.position.x, slingShotInitPosition.position.y, slingShotInitPosition.position.z);

        _slingShot = _characterController.SlingShotPooler.Pull<ISlingShot>(type, fixedInitPosition, Quaternion.Euler(90, 0, 0), slingShotInitPosition.parent);

        _slingShot.Init(slingShotInitPosition.position, type, _characterController.GetCurrentLaunchDistance());

        _slingShot.ON_DIRECTION_CHANGE -= _characterController.CharacterView.ChangeDirection;
        _slingShot.ON_DIRECTION_CHANGE += _characterController.CharacterView.ChangeDirection;
        IAbility currentAbility = _characterController.CurrentAbility;
        if (currentAbility.ProjectileType == ProjectileType.None)
        {
            _slingShot.ON_SHOOT -= LaunchProjectile;
            _slingShot.ON_SHOOT -= LaunchYourself;
            _slingShot.ON_SHOOT += LaunchYourself;
        }
        else
        {
            _slingShot.ON_SHOOT -= LaunchYourself;
            _slingShot.ON_SHOOT -= LaunchProjectile;
            _slingShot.ON_SHOOT += LaunchProjectile;
        }

        _slingShot.ON_ABILITY_USE -= _characterController.ProcessReloadAbility;
        _slingShot.ON_ABILITY_USE += _characterController.ProcessReloadAbility;

        DragInputModule.dragFocusObject = _slingShot.gameObject;
        eventData.pointerDrag = _slingShot.gameObject;
        eventData.dragging = true;
    }

    public override void LaunchYourself(Vector3 direction)
    {
        _playerController.PlayerAttackWaitingAnimation.Stop(() => { });

        float slingshotRadius = 2.1f;
        float slingshotMultiplier = direction.magnitude / slingshotRadius;
        Vector3 forceVector = GetForceVector(direction);
        _characterController.GetRigidbody().velocity = forceVector * slingshotMultiplier;

        _slingShot.ON_SHOOT -= LaunchYourself;

    }

    public override void LaunchProjectile(Vector3 direction)
    {
        base.LaunchProjectile(direction);
        _slingShot.ON_SHOOT -= LaunchProjectile;
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