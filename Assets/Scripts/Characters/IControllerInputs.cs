using CharactersStats;
using Cinemachine;
using Interactions;
using Projectiles;
using System.Collections.Generic;
using UnityEngine;


public interface IControllerInputs
{
    public bool IsMoving { get; set; }
    public bool IsActive { get; set; }
    public List<IProjectile> LaunchedProjectiles { get; set; }
    public IPoolManager PoolManager { get; set; }
    public ICameraManager CameraManager { get; set; }
    public void SetAnimator(Animator animator);
    public ReactiveStats GetCharacterStats();
    public IInteraction GetInteraction();
    public IConditionState GetCurrentConditionState();
    public void ApplyInteraction(IInteraction interactions);
    public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer);
    public void AddEffects(List<IEffect> effects);
    public bool GetActiveStatus();
    public void DoUpdate();
    public void PushCharacterUI();
    public Transform GetTransform();

    public void HandleStopMovement();
}
