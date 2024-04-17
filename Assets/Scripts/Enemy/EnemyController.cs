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

namespace Enemy
{
    public interface IEnemyController : ICharacterController
    {
        public void OnClick(Transform point, PointerEventData eventData);
    }
    public class EnemyController : IEnemyController, IControllerInputs, IDisposable
    {
        public event OnCharacterDeath ON_CHARACTER_DEATH;
        public event OnStopMovement ON_STOP_MOVEMENT;
        public bool IsActive { get; set; }
        public bool IsStunned { get; set; }
        public bool IsMoving { get; set; }
        public IAnalyzer Analyzer { get; set; }
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

        private IConditionState _currentState;
        private IStateFactory _stateFactory;
        private ICharacterScenarioContext _characterScenarioContext;
        private Transform _slingShotInitPosition;
        private CharacterPooler _pooler;
        public NavMeshObstacle NavMeshObstacle { get; set; }
        private DiContainer _container;

        [Inject]
        public void Construct(
            IInteractionProcessor interactionProcessor,
            IInteractionDealer interactionDealer,
            IEffectProcessor effector,
            IInteractionCalculator interactionFinalizer,
            IStateFactory stateFactory,
            DiContainer container)
        {
            InteractionProcessor = interactionProcessor;
            InteractionDealer = interactionDealer;
            Effector = effector;
            InteractionCalculator = interactionFinalizer;
            _stateFactory = stateFactory;
            _container = container;
        }

        public void Init(CharacterModel characterModel, CharacterView characterView, CharacterPooler characterPooler)
        {
            DefaultBehaviourTree = _container.Resolve<IDefaultBehaviourTree>();
            DefaultBehaviourTree.InitTree(this);

            CharacterModel = characterModel;

            var stats = CharacterModel.GetStats();
            ModifiableStats = stats.ToReactive();

            _currentState = _stateFactory.CreateConditionState(TypeOfConditionState.InactiveState, this);
            Analyzer = new Analyzer(this);

            InteractionDealer.Init(ModifiableStats);

            CharacterView = characterView;
            _pooler = characterPooler;
            CharacterView.Init(this);
            NavMeshAgent = CharacterView.NavMeshAgent;
            NavMeshAgent.enabled = false;
            CharacterView.ON_CLICK += OnClick;
            ON_STOP_MOVEMENT += CheckForEndOfState;

            NavMeshObstacle = CharacterView.NavMeshObstacle;
            NavMeshObstacle.carving = true;
            NavMeshObstacle.carveOnlyStationary = true;
        }

        public void DoUpdate()
        {
            _currentState.DoUpdate();

            
        }

        public void OnClick(Transform point, PointerEventData eventData)
        {
            _slingShotInitPosition = point;
            //Debug.Log($"-----|{CharacterModel.Type}|-----");
            //Debug.Log("<color=#189C0C>" + "Hp: " + ModifiableStats.Health.Value + "</color>");

            //Debug.Log("<color=#F4DA64>" + "Effects Before interaction: " + "</color>");
            //Effector.PrintEffects(Effector.GetPreInteractionEffects());

            //Debug.Log("<color=#F4DA64>" + "Effects on Start turn: " + "</color>");
            //Effector.PrintEffects(Effector.GetOnStartTurnInteractionEffects());

            //Debug.Log("<color=#F4DA64>" + "Effects on End turn: " + "</color>");
            //Effector.PrintEffects(Effector.GetOnEndTurnInteractionEffects());
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsActive && !IsMoving)
            {
                _currentState.UseSlingshot(eventData, _slingShotInitPosition);
            }
        }
        public IInteraction GetInteraction()
        {
            return _currentState.GetInteraction(InteractionType.BasicAttack);
        }

        public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer)
        {
            if (IsMoving)
            {
                _currentState.ApplyBump(interactible, bumpFromDealer);
            }
        }

        public void HandleStopMovement()
        {
            ON_STOP_MOVEMENT?.Invoke();
        }
        public void Dispose()
        {
            CharacterView.ON_CLICK -= OnClick;
        }

        public ReactiveStats GetCharacterStats()
        {
            return ModifiableStats;
        }

        public void ApplyInteraction(IInteraction interaction)
        {
            _currentState.ApplyInteraction(interaction);
        }

        public void AddEffects(List<IEffect> effects)
        {
            _currentState.AddEffects(effects);
        }
        public void Launch(Vector2 direction)
        {
            _currentState.Launch(direction);
        }

        public void LaunchToPoint(Vector3 point)
        {
            _currentState.LaunchToPoint(point);
        }

        public void CheckForEndOfState()
        {
            _characterScenarioContext.CheckIfAllStopped();
        }

        public void PushIfDead()
        {
            ON_CHARACTER_DEATH?.Invoke(this);
            _pooler.Push(CharacterModel.Type, CharacterView);
        }

        public bool CheckIfMoving()
        {
            return IsMoving;
        }

        public void Attack()
        {
            _currentState.Attack();
        }

        public void Move()
        {
            _currentState.Move();
        }

        public void Tick()
        {
            DefaultBehaviourTree.TickTree();
        }

        public void SkipTurn()
        {
            ON_STOP_MOVEMENT?.Invoke();
        }

        public void SetCharacterContext(ICharacterScenarioContext characterScenarioContext)
        {
            _characterScenarioContext = characterScenarioContext;
            DefaultBehaviourTree.SetCharacters(_characterScenarioContext);
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
            //Debug.LogWarning("Effects On Start Was Processed:");
        }

        public void UseEffectsOnEnd()
        {
            Effector.ProcessEffectsOnEnd(ModifiableStats);
            //Debug.LogWarning("Effects On End Was Processed!");
        }

        public void AnalizeCondition()
        {
            //Debug.Log("<color=#9C5F62>" + "--|Analyzing condition|-- " + "</color>");
            Analyzer.Analyze(ModifiableStats, Effector);
        }

        public void SwitchState(TypeOfConditionState state)
        {
            IConditionState newState = _stateFactory.CreateConditionState(state, this);

            if (!_currentState.Equals(newState))
            {
                _currentState?.OnExit();
                _currentState = newState;
                _currentState.OnEnter();
            } 
        }

        public CharacterType GetCharacterType()
        {
            return CharacterModel.Type;
        }

        public IConditionState GetCurrentConditionState()
        {
            return _currentState;
        }
    }
}