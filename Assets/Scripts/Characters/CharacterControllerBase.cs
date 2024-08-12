using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Interactions;
using System.Collections.Generic;
using Pool;
using BehaviourTree;
using CharactersStats;
using Zenject;
using Gameplay;
using UnityEngine.AI;
using Abilities;
using Projectiles;
using Cinemachine;

public delegate void OnEndTurn();
public abstract class CharacterControllerBase : ICharacterController, IControllerInputs, IDisposable
{
    public event OnCharacterDeath ON_CHARACTER_DEATH;
    public event OnStopMovement ON_STOP_MOVEMENT;

    public AnimationBase MoveAnimation { get; set; }
    public bool IsActive { get; set; }
    public bool IsStunned { get; set; }
    public bool IsMoving { get; set; }
    public bool IsDead { get; set; }
    public IPoolManager PoolManager { get; set; }
    public IAnalyzer Analyzer { get; set; }
    public IAbility CurrentAbility { get; set; }
    public IEffectProcessor Effector { get; set; }
    public IInteractionProcessor InteractionProcessor { get; set; }
    public IInteractionDealer InteractionDealer { get; set; }
    public IInteractionCalculator InteractionCalculator { get; set; }
    public IDefaultBehaviourTree DefaultBehaviourTree { get; set; }
    public CharacterView CharacterView { get; set; }
    public CharacterModel CharacterModel { get; set; }
    public SlingshotPooler SlingShotPooler { get; set; }
    public ReactiveStats ModifiableStats { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    public NavMeshObstacle NavMeshObstacle { get; set; }
    public List<IProjectile> LaunchedProjectiles { get; set; }
    public CinemachineVirtualCamera VirtualCamera { get; set; }
    public ICameraManager CameraManager { get; set; }

    protected IConditionState CurrentState;
    protected IStateFactory StateFactory;
    protected IRoomContext CharacterScenarioContext;
    protected IUIFactory UIFactory;
    protected CharacterUIView UIView;
    protected ReactiveList<IEffect> AllEffects;
    protected Transform SlingShotInitPosition;
    protected CharacterPooler CharacterPooler;
    protected CharacterUIPooler CharacterUIPooler;
    protected CharacterUIViewmodel UIViewmodel;
    protected DiContainer Container;
    protected IGameplayService GameplayService;
    
    protected Animator Animator;

    public CharacterControllerBase(
        IPoolManager poolManager,
        IInteractionProcessor interactionProcessor,
        IInteractionDealer interactionDealer,
        IEffectProcessor effector,
        IInteractionCalculator interactionFinalizer,
        IStateFactory stateFactory,
        IUIFactory uIFactory,
        DiContainer container,
        IGameplayService gameplayService,
        ICameraManager cameraManager)
    {
        PoolManager = poolManager;
        CharacterUIPooler = poolManager.UseCharacterUIPooler();
        CharacterPooler = poolManager.UseCharacterPooler();
        InteractionProcessor = interactionProcessor;
        InteractionDealer = interactionDealer;
        Effector = effector;
        InteractionCalculator = interactionFinalizer;
        StateFactory = stateFactory;
        UIFactory = uIFactory;
        Container = container;
        GameplayService = gameplayService;
        CameraManager = cameraManager;
    }

    public abstract void Init(CharacterModel characterModel, CharacterView characterView, CharacterUIView characterUIView);

    public virtual void DoUpdate()
    {
        CurrentState.DoUpdate();
    }

    public virtual void SetAnimator(Animator animator)
    {
        Animator = animator;
    }

    public virtual void SetVirtualCamera(CinemachineVirtualCamera VC)
    {
        VirtualCamera = VC;
    }

    public virtual CinemachineVirtualCamera GetVirtualCamera()
    {
        return VirtualCamera;
    }

    public abstract void SetCurrentAbility(IAbility ability);
    public abstract void OnClick(Transform point, PointerEventData eventData);
    public abstract void OnBeginDrag(PointerEventData eventData);

    public virtual float GetCurrentLaunchDistance()
    {
        float launchPower = ModifiableStats.LaunchPower.Value;
        float dragConstant = GetRigidbody().drag;
        float maxDistance = launchPower / dragConstant;
        float currentLaunchDistance = maxDistance * CurrentAbility.GetLaunchModifier();
        return currentLaunchDistance;
    }

    public virtual IInteraction GetInteraction()
    {
        return CurrentState.GetInteraction();
    }

    public virtual void ApplyBump(IInteractible interactible, IMovable bumpFromDealer)
    {
        if (interactible is IProjectile)
        {
            CurrentState.ApplyBump(interactible, bumpFromDealer);
        }
        else if (IsActive)
        {
            CurrentState.ApplyBump(interactible, bumpFromDealer);
        }
    }

    public virtual void HandleStopMovement()
    {
        ON_STOP_MOVEMENT?.Invoke();
    }

    public virtual void Dispose()
    {
        CharacterView.ON_CLICK -= OnClick;
        ON_STOP_MOVEMENT -= CheckForEndOfState;
    }

    public virtual ReactiveStats GetCharacterStats()
    {
        return ModifiableStats;
    }

    public virtual void ApplyInteraction(IInteraction interaction)
    {
        CurrentState.ApplyInteraction(interaction);
        UIViewmodel.UpdateStats(ModifiableStats);
        UIViewmodel.VisualiseEffects(AllEffects.Value);
    }

    public virtual void AddEffects(List<IEffect> effects)
    {
        CurrentState.AddEffects(effects);
        UIViewmodel.VisualiseEffects(AllEffects.Value);
    }

    public virtual void CheckForEndOfState()
    {
        GameplayService.CheckIfAllStopped();
    }

    public virtual void PushIfDead()
    {
        CharacterPooler.Push(CharacterModel.Type, CharacterView);
        PushCharacterUI();
        ON_CHARACTER_DEATH?.Invoke(this);
    }

    public virtual bool CheckIfMoving()
    {
        return IsMoving;
    }

    public virtual void Attack()
    {
        CurrentState.Attack();
    }

    public virtual void Move()
    {
        CurrentState.Move();
    }

    public virtual void Tick()
    {
        DefaultBehaviourTree.TickTree();
    }

    public virtual void SetCharacterContext(IRoomContext characterScenarioContext)
    {
        CharacterScenarioContext = characterScenarioContext;
        DefaultBehaviourTree.SetCharacters(CharacterScenarioContext);
    }

    public virtual Transform GetTransform()
    {
        return CharacterView.GetTransform();
    }

    public virtual Transform GetProjectileSpawn()
    {
        return CharacterView.GetProjectileSpawn();
    }

    public virtual bool GetActiveStatus()
    {
        return IsActive;
    }

    public virtual void UseEffectsOnStart()
    {
        Effector.ProcessEffectsOnStart(ModifiableStats);
    }

    public virtual void UseEffectsOnEnd()
    {
        Effector.ProcessEffectsOnEnd(ModifiableStats);
    }

    public virtual void AnalizeCondition()
    {
        Analyzer.Analyze(ModifiableStats, Effector);
    }

    public virtual void SwitchState(TypeOfConditionState state)
    {
        IConditionState newState = StateFactory.CreateConditionState(state, this);

        if (!CurrentState.Equals(newState))
        {
            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState.OnEnter();
        }

        if (state == TypeOfConditionState.DeadState)
        {
            IsDead = true;
        }
    }

    public virtual CharacterType GetCharacterType()
    {
        return CharacterModel.Type;
    }

    public virtual IConditionState GetCurrentConditionState()
    {
        return CurrentState;
    }

    public virtual Rigidbody GetRigidbody()
    {
        return CharacterView.GetRigidbody();
    }

    public virtual Vector3 GetVelocity()
    {
        return CharacterView.GetVelocity();
    }

    public virtual void PushCharacterUI()
    {
        CharacterUIPooler.Push(UIType.CharacterUI, UIView);
    }

    public virtual void ActivateUI()
    {
        UIView.gameObject.SetActive(true);
    }

    public virtual void DisableUI()
    {
        UIView.gameObject.SetActive(false);
    }

    public virtual void UpdateEffectsOnUI(List<IEffect> displayedEffects)
    {
        UIViewmodel.VisualiseEffects(displayedEffects);
    }

    public virtual void UpdateHealthBar()
    {
        UIViewmodel.UpdateHealthBar();
    }

    public abstract void ProcessReloadAbility();

    public virtual void ProcessOnStartTurn()
    {
        UIViewmodel.ToggleActiveIndicator();
        CameraManager.SetMainCamera(VirtualCamera);
    }

    public abstract void RevertReadyUnactiveAbilityButtons();

    public virtual void ProcessOnEndTurn()
    {
        UIViewmodel.ToggleActiveIndicator();
    }
}
