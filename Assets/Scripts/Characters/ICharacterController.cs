using BehaviourTree;
using CharactersStats;
using Gameplay;
using Interactions;
using Pool;
using UnityEngine;
using UnityEngine.AI;

public interface ICharacterController
{
    public bool IsActive { get; set; }
    public bool IsStunned { get; set; }
    public bool IsMoving { get; set; }
    public IEffectProcessor Effector { get; set; }
    public IInteractionProcessor InteractionProcessor { get; set; }
    public IInteractionDealer InteractionDealer { get; set; }
    public IInteractionCalculator InteractionCalculator { get; set; }
    public IDefaultBehaviourTree DefaultBehaviourTree { get; set; }
    public IAbility CurrentAbility { get; set; }
    public IAnalyzer Analyzer { get; set; }
    public ReactiveStats ModifiableStats { get; set; }
    public CharacterView CharacterView { get; set; }
    public SlingshotPooler SlingShotPooler { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    public NavMeshPath Path { get; set; }
    public CharacterModel CharacterModel { get; set; }
    public NavMeshObstacle NavMeshObstacle { get; set; }
    public void ProcessReloadAbility();
    public void ProcessOnStartTurn();
    public void ProcessOnEndTurn();
    public void SetCurrentAbility(IAbility ability);
    public void UpdateHealthBar();
    public CharacterType GetCharacterType();
    public Transform GetTransform();
    public Rigidbody GetRigidbody();
    public void Init(CharacterModel model, CharacterView playerView, CharacterPooler pooler, CharacterUIView uIView);
    public Vector3 GetVelocity();
    public void ActivateUI();
    public void DisableUI();
    public void UseEffectsOnStart();
    public void AnalizeCondition();
    public void SwitchState(TypeOfConditionState state);
    public void UseEffectsOnEnd();
    public void PushIfDead();
    public void JustPush();
    public void Attack();
    public void Move();
    public void Tick();
    public bool CheckIfMoving();
    public void SetCharacterContext(ICharacterScenarioContext characterScenarioContext);
    public void HandleStopMovement();

    public event OnCharacterDeath ON_CHARACTER_DEATH;
    public event OnStopMovement ON_STOP_MOVEMENT;
}

public delegate void OnCharacterDeath(ICharacterController controller);
public delegate void OnStopMovement();
