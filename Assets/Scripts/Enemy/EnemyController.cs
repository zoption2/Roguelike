using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Interactions;
using System.Collections.Generic;
using Pool;
using BehaviourTree;
using CharactersStats;
using Zenject;
using System.Threading.Tasks;
using Gameplay;
using Prefab;
using Obstacles;
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
        private IConditionState _conditionState;
        private IStateFactory _stateFactory;
        private ICharacterScenarioContext _characterScenarioContext;
        private ITestingBehaviourTree _testBehaviourTree;
        public IEffectProcessor Effector { get; set; }
        public IInteractionProcessor InteractionProcessor { get; set; }
        public IInteractionDealer InteractionDealer { get; set; }
        public IInteractionCalculator InteractionFinalizer { get; set; }
        public CharacterView CharacterView { get; set; }
        private CharacterModel _enemyModel;
        public ReactiveStats ModifiableStats { get; set; }
        private ReactiveStats _interactionResult;
        private CharacterPooler _pooler;
        private NavMeshAgent _navMeshAgent;
        private DiContainer _container;
        private int _milisecondsDelay = 3000;

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
            InteractionFinalizer = interactionFinalizer;
            _stateFactory = stateFactory;
            _container = container;
        }

        public void Init(CharacterModel characterModel, CharacterView characterView, CharacterPooler characterPooler)
        {
            _testBehaviourTree = _container.Resolve<ITestingBehaviourTree>();
            _testBehaviourTree.InitTree(this);

            _enemyModel = characterModel;

            var stats = _enemyModel.GetStats();
            ModifiableStats = stats.ToReactive();

            _conditionState = _stateFactory.CreateConditionState(TypeOfConditionState.InactiveState, this);
            Analyzer = new Analyzer(this);

            InteractionDealer.Init(ModifiableStats);

            CharacterView = characterView;
            _pooler = characterPooler;
            CharacterView.Init(this);
            _navMeshAgent = CharacterView.NavMeshAgent;
            _navMeshAgent.enabled = false;
            CharacterView.ON_CLICK += OnClick;
            ON_STOP_MOVEMENT += CheckForEndOfState;
        }

        public void DoUpdate()
        {
            if (CharacterView.Rigidbody.velocity.magnitude > CharacterView.MaxVelocity)
            {
                CharacterView.Rigidbody.velocity = CharacterView.Rigidbody.velocity.normalized * CharacterView.MaxVelocity;
            }


            if (CharacterView.Rigidbody.velocity.magnitude > 0.5f && !IsMoving)
            {
                IsMoving = true;
            }
            else if (CharacterView.Rigidbody.velocity.magnitude < 0.2f && CharacterView.Rigidbody.velocity.magnitude > 0f && IsMoving)
            {
                IsMoving = false;
                ON_STOP_MOVEMENT?.Invoke();
            }

            if (IsMoving)
            {
                _conditionState.ViewRotation();
            }
        }

        public void OnClick(Transform point, PointerEventData eventData)
        {
            Debug.Log($"-----|{_enemyModel.Type}|-----");
            Debug.Log("<color=#189C0C>" + "Hp: " + ModifiableStats.Health.Value + "</color>");

            Debug.Log("<color=#F4DA64>" + "Effects Before interaction: " + "</color>");
            Effector.PrintEffects(Effector.GetPreInteractionEffects());

            Debug.Log("<color=#F4DA64>" + "Effects on Start turn: " + "</color>");
            Effector.PrintEffects(Effector.GetOnStartTurnInteractionEffects());

            Debug.Log("<color=#F4DA64>" + "Effects on End turn: " + "</color>");
            Effector.PrintEffects(Effector.GetOnEndTurnInteractionEffects());
        }

        public IInteraction GetInteraction()
        {
            return _conditionState.GetInteraction(InteractionType.BasicAttack);
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
            _conditionState.ApplyInteraction(interaction);
        }

        public void AddEffects(List<IEffect> effects)
        {
            _conditionState.AddEffects(effects);
        }
        public void Launch(Vector2 direction)
        {
            _conditionState.Launch(direction);
        }

        public void LaunchToPoint(Vector3 point)
        {
            float launchPower = ModifiableStats.LaunchPower.Value;
            Vector3 direction = point - GetTransform().position;
            float distance = direction.magnitude;
            direction.Normalize();
            float multiplier = Mathf.Clamp(distance, 4, launchPower);
            Vector3 initialVelocity = direction * multiplier;
            CharacterView.Rigidbody.AddForce(initialVelocity, ForceMode.VelocityChange);
        }

        public void CheckForEndOfState()
        {
            _characterScenarioContext.CheckIfAllStopped();
        }

        public void PushIfDead()
        {
            ON_CHARACTER_DEATH?.Invoke(this);
            _pooler.Push(_enemyModel.Type, CharacterView);
        }

        public bool CheckIfMoving()
        {
            return IsMoving;
        }

        public async void Attack()
        {
            Transform target = _testBehaviourTree.GetTarget();
            Transform enemy = GetTransform();
            Vector2 direction = enemy.position - target.position;
            CharacterView.ChangeDirection(-direction);
            await Task.Delay(_milisecondsDelay);
            if(_navMeshAgent != null)
                _navMeshAgent.enabled = false;
            Launch(direction * -1);
        }

        public async void Move()
        {
            _navMeshAgent.enabled = true;
            Transform target = _testBehaviourTree.GetTarget();
            Transform enemy = GetTransform();
            _navMeshAgent.SetDestination(target.position);
            _navMeshAgent.isStopped = true;
            await Task.Delay(_milisecondsDelay/10);
            Vector3 waypoint = _navMeshAgent.steeringTarget;
            _navMeshAgent.enabled = false;
            Vector2 direction = enemy.position - waypoint;
            CharacterView.ChangeDirection(-direction);
            await Task.Delay(_milisecondsDelay);
            LaunchToPoint(waypoint);
        }

        public void Tick()
        {
            _testBehaviourTree.TickTree();
        }

        public void SkipTurn()
        {
            ON_STOP_MOVEMENT?.Invoke();
        }

        public void SetCharacterContext(ICharacterScenarioContext characterScenarioContext)
        {
            _characterScenarioContext = characterScenarioContext;
            _testBehaviourTree.SetCharacters(_characterScenarioContext);
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
            Debug.LogWarning("Effects On Start Was Processed:");
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
            return _enemyModel.Type;
        }

        public IConditionState GetCurrentConditionState()
        {
            return _conditionState;
        }
    }
}