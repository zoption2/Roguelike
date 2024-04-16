using BehaviourTree;
using CharactersStats;
using Gameplay;
using Interactions;
using Pool;
using UnityEngine;
using UnityEngine.AI;

public interface ICharacterController
{
    public IEffectProcessor Effector { get; set; }
    public IInteractionProcessor InteractionProcessor { get; set; }
    public IInteractionDealer InteractionDealer { get; set; }
    public IInteractionCalculator InteractionFinalizer { get; set; }
    public ITestingBehaviourTree TestBehaviourTree { get; set; }
    public ReactiveStats ModifiableStats { get; set; }
    public IAnalyzer Analyzer { get; set; }
    public CharacterView CharacterView { get; set; }
    public SlingshotPooler SlingShotPooler { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    public bool IsActive { get; set; }
    public bool IsStunned { get; set; }
    public CharacterModel CharacterModel { get; set; }
    public bool IsMoving { get; set; }
    public void Init(CharacterModel model, CharacterView playerView, CharacterPooler pooler);
    public void UseEffectsOnStart();
    public void AnalizeCondition();
    public void SwitchState(TypeOfConditionState state);
    public void UseEffectsOnEnd();
    public CharacterType GetCharacterType();
    public void PushIfDead();
    public void SkipTurn();
    public void Attack();
    public void Move();
    public void Tick();
    public bool CheckIfMoving();
    public void SetCharacterContext(ICharacterScenarioContext characterScenarioContext);
    public Transform GetTransform();
    public void HandleStopMovement();


    public event OnCharacterDeath ON_CHARACTER_DEATH;
    public event OnStopMovement ON_STOP_MOVEMENT;
}

public delegate void OnCharacterDeath(ICharacterController controller);
public delegate void OnStopMovement();
