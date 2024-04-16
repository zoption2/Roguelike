using CharactersStats;
using Interactions;
using Gameplay;
using Pool;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using UnityEngine.AI;
using BehaviourTree;

namespace Player
{
    public interface IPlayerController : ICharacterController
    {
        public void OnClick(Transform point, PointerEventData eventData);
        public void OnBeginDrag(PointerEventData eventData);
    }

    public delegate void OnEndTurn();
    public class PlayerController : IPlayerController, IControllerInputs, IDisposable
    {
        public event OnCharacterDeath ON_CHARACTER_DEATH;
        public event OnStopMovement ON_STOP_MOVEMENT;
        public bool IsActive { get; set; }
        public bool IsStunned { get; set; }
        public bool IsMoving { get; set; }
        public CharacterView CharacterView { get; set; }
        public CharacterModel CharacterModel { get; set; }
        public SlingshotPooler SlingShotPooler { get; set; }
        public ReactiveStats ModifiableStats { get; set; }
        public NavMeshAgent NavMeshAgent { get; set; }
        public IInteractionProcessor InteractionProcessor { get; set; }
        public IInteractionDealer InteractionDealer { get; set; }
        public IInteractionCalculator InteractionFinalizer { get; set; }
        public ITestingBehaviourTree TestBehaviourTree { get; set; }
        public IEffectProcessor Effector { get; set; }
        public IAnalyzer Analyzer { get; set; }

        private Transform _slingShotInitPosition;
        private CharacterPooler _pooler;
        private DiContainer _container;
        private IConditionState _conditionState;
        private IStateFactory _stateFactory;
        private ICharacterScenarioContext _characterScenarioContext;
        
        [Inject]
        public void Construct(
            SlingshotPooler slingShotPooler,      
            IInteractionProcessor interactionProcessor,
            IInteractionDealer interactionDealer,
            IEffectProcessor effector,
            IInteractionCalculator interactionFinalizer,
            IStateFactory stateFactory,
            DiContainer container)
        {
            SlingShotPooler = slingShotPooler;
            InteractionProcessor = interactionProcessor;
            InteractionDealer = interactionDealer;
            Effector = effector;
            InteractionFinalizer = interactionFinalizer; 
            _stateFactory = stateFactory; 
            _container = container;
        }

        public void Init(
        CharacterModel playerModel,
        CharacterView playerView,
        CharacterPooler characterPooler)
        {
            TestBehaviourTree = _container.Resolve<ITestingBehaviourTree>();
            TestBehaviourTree.InitTree(this);

            CharacterModel = playerModel;

            var stats = CharacterModel.GetStats();
            ModifiableStats = stats.ToReactive();

            _conditionState = _stateFactory.CreateConditionState(TypeOfConditionState.InactiveState, this);
            Analyzer = new Analyzer(this);

            CharacterView = playerView;
            _pooler = characterPooler;
            CharacterView.Init(this);
            NavMeshAgent = CharacterView.NavMeshAgent;
            NavMeshAgent.enabled = false;
            _navMeshObstacle = _playerView.NavMeshObstacle;
            _navMeshObstacle.carving = true;
            _navMeshObstacle.carveOnlyStationary = true;

            CharacterView.ON_CLICK += OnClick;
            CharacterView.ON_BEGINDRAG += OnBeginDrag;
            ON_STOP_MOVEMENT += CheckForEndOfState;
            SlingShotPooler.Init();
        }

        public void DoUpdate()
        {
            _conditionState.DoUpdate();
        }



        public void OnClick(Transform point, PointerEventData eventData)
        {
            _slingShotInitPosition = point;

            Debug.Log($"-----|{CharacterModel.Type}|-----");
            Debug.Log("<color=#189C0C>" + "Hp: " + ModifiableStats.Health.Value + "</color>");

            Debug.Log("<color=#F4DA64>" + "Effects Before interaction: " + "</color>");
            Effector.PrintEffects(Effector.GetPreInteractionEffects());

            Debug.Log("<color=#F4DA64>" + "Effects on Start turn: " + "</color>");
            Effector.PrintEffects(Effector.GetOnStartTurnInteractionEffects());

            Debug.Log("<color=#F4DA64>" + "Effects on End turn: " + "</color>");
            Effector.PrintEffects(Effector.GetOnEndTurnInteractionEffects());
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!IsMoving)
            {
                _conditionState.UseSlingshot(eventData, _slingShotInitPosition);
            }
        }

        public void Launch(Vector2 direction)
        {
            _conditionState.Launch(direction);
        }

        public IInteraction GetInteraction()
        {
            return _conditionState.GetInteraction(InteractionType.Knight_HeavyAttack);
        }

        public void ApplyInteraction(IInteraction interaction)
        {
            _conditionState.ApplyInteraction(interaction);
        }

        public void CheckForEndOfState()
        {
            _characterScenarioContext.CheckIfAllStopped();
        }

        public void PushIfDead()
        {
            ON_CHARACTER_DEATH(this);
            _pooler.Push(CharacterModel.Type, CharacterView);
        }

        public ReactiveStats GetCharacterStats()
        {
            return ModifiableStats;
        }

        public void AddEffects(List<IEffect> effects)
        {
            _conditionState.AddEffects(effects);
        }

        public void Attack()
        {
            _conditionState.Attack();
        }

        public void Move()
        {
            _conditionState.Move();
        }

        public void Tick()
        {
        }

        public void SkipTurn()
        {
            ON_STOP_MOVEMENT?.Invoke();
        }

        public bool CheckIfMoving()
        {
            return IsMoving;
        }
        public void SetCharacterContext(ICharacterScenarioContext characterScenarioContext)
        {
            _characterScenarioContext = characterScenarioContext;
        }

        public Transform GetTransform()
        {
            return CharacterView.GetTransform();
        }

        public bool GetActiveStatus()
        {
            return IsActive;
        }

        public void UseEffectsOnStart()
        {
            Effector.ProcessEffectsOnStart(ModifiableStats);
            Debug.LogWarning("Effects On Start Was Processed!");
        }

        public void UseEffectsOnEnd()
        {
            Effector.ProcessEffectsOnEnd(ModifiableStats);
            Debug.LogWarning("Effects On End Was Processed!");
        }

        public void AnalizeCondition()
        {
            Debug.Log("<color=#9C5F62>" + "--|Analyzing condition|-- " + "</color>");
            Analyzer.Analyze(ModifiableStats, Effector);
        }

        public void SwitchState(TypeOfConditionState state)
        {
            IConditionState newState = _stateFactory.CreateConditionState(state, this);

            if (!_conditionState.Equals(newState))
            {
                _conditionState?.OnExit();
                _conditionState = newState;
                _conditionState.OnEnter();
            }
        }

        public CharacterType GetCharacterType()
        {
            return CharacterModel.Type;
        }

        public void HandleStopMovement()
        {
            ON_STOP_MOVEMENT?.Invoke();
        }
        public IConditionState GetCurrentConditionState()
        {
            return _conditionState;
        }
        public void Dispose()
        {
            CharacterView.ON_CLICK -= OnClick;
            CharacterView.ON_BEGINDRAG -= OnBeginDrag;
        }
    }
}
