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
        public bool IsActive { get; set; }

        private IEffectProcessor _effector;
        private IAnalyzer _analyzer;
        private IConditionState _conditionState;
        private IStateFactory _stateFactory;
        private ICharacterScenarioContext _characterScenarioContext;
        private ITestingBehaviourTree _testBehaviourTree;
        private IInteractionProcessor _interactionProcessor;
        private IInteractionDealer _interactionDealer;
        private IInteractionCalculator _interactionFinalizer;
        private CharacterView _enemyView;
        private CharacterModel _enemyModel;
        private ReactiveStats _modifiableStats;
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
            _interactionProcessor = interactionProcessor;
            _interactionDealer = interactionDealer;
            _effector = effector;
            _interactionFinalizer = interactionFinalizer;
            _stateFactory = stateFactory;
            _container = container;
        }

        public void Init(CharacterModel characterModel, CharacterView characterView, CharacterPooler characterPooler)
        {
            _testBehaviourTree = _container.Resolve<ITestingBehaviourTree>();
            _testBehaviourTree.InitTree(this);

            _enemyModel = characterModel;

            var stats = _enemyModel.GetStats();
            _modifiableStats = stats.ToReactive();

            _conditionState = _stateFactory.CreateConditionState(TypeOfConditionState.DefaultState, this);
            _analyzer = new Analyzer(this);

            _interactionDealer.Init(_modifiableStats);

            _enemyView = characterView;
            _pooler = characterPooler;
            _enemyView.Init(this);
            _navMeshAgent = _enemyView.NavMeshAgent;
            _navMeshAgent.enabled = false;
            _enemyView.ON_CLICK += OnClick;
            _enemyView.ON_STOP_MOVEMENT += CheckForEndOfState;
        }

        public void OnClick(Transform point, PointerEventData eventData)
        {
            Debug.Log($"-----|{_enemyModel.Type}|-----");
            Debug.Log("<color=#189C0C>" + "Hp: " + _modifiableStats.Health.Value + "</color>");

            Debug.Log("<color=#F4DA64>" + "Effects Before interaction: " + "</color>");
            _effector.PrintEffects(_effector.GetPreInteractionEffects());

            Debug.Log("<color=#F4DA64>" + "Effects on Start interaction: " + "</color>");
            _effector.PrintEffects(_effector.GetOnStartTurnInteractionEffects());

            Debug.Log("<color=#F4DA64>" + "Effects on End interaction: " + "</color>");
            _effector.PrintEffects(_effector.GetOnEndTurnInteractionEffects());
        }

        public IInteraction GetInteraction()
        {
            IInteraction interaction;
            if (IsActive)
            {
                ReactiveStats statsWithBonus = _effector.ProcessStatsBeforeInteraction(_modifiableStats);

                _interactionDealer.Init(statsWithBonus);
                interaction = _interactionDealer.UseInteraction(InteractionType.BasicAttack);
                return interaction;
            }
            else
            {
                interaction = _interactionDealer.UseInteraction(InteractionType.None);
                return interaction;
            }
        }

        public void Dispose()
        {
            _enemyView.ON_CLICK -= OnClick;
        }

        public ReactiveStats GetCharacterStats()
        {
            return _modifiableStats;
        }

        public void ApplyInteraction(IInteraction interaction)
        {
            if (!IsActive)
            {
                List<IEffect> effects = interaction.GetEffects();
                if (effects != null && effects.Count > 0)
                {
                    foreach (IEffect effect in effects)
                    {
                        _effector.AddEffects(effects);
                    }
                }
                _interactionResult = _interactionProcessor.ProcessInteraction(interaction);

                _modifiableStats = _interactionFinalizer.CalculateInteractionResult(_modifiableStats, _interactionResult);
            }
        }

        public void AddEffects(List<IEffect> effects)
        {
            if (effects != null && effects.Count > 0)
            {
                foreach (IEffect effect in effects)
                {
                    _effector.AddEffects(effects);
                }
            }
        }
        public void Launch(Vector2 direction)
        {
            float launchPower = _modifiableStats.LaunchPower.Value;
            direction.Normalize();
            Vector2 forceVector = direction * launchPower;
            _enemyView.Rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
        }

        public void CheckForEndOfState()
        {
            _characterScenarioContext.CheckIfAllStopped();
        }

        public void PushIfDead()
        {
            ON_CHARACTER_DEATH?.Invoke(this);
            _pooler.Push(_enemyModel.Type, _enemyView);
        }

        public bool CheckIfMoving()
        {
            return _enemyView.IsMoving;
        }

        public async void Attack()
        {
            Transform target = _testBehaviourTree.GetTarget();
            Transform enemy = GetTransform();
            Vector2 direction = enemy.position - target.position;
            _enemyView.ChangeDirection(-direction);
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
            _enemyView.ChangeDirection(-direction);
            await Task.Delay(_milisecondsDelay);
            Launch(direction * -1);
        }

        public void Tick()
        {
            _testBehaviourTree.TickTree();
        }

        public async void SkipTurn()
        {
            await Task.Delay(_milisecondsDelay);
            _enemyView.TrySkipTurn();
        }

        public void SetCharacterContext(ICharacterScenarioContext characterScenarioContext)
        {
            _characterScenarioContext = characterScenarioContext;
            _testBehaviourTree.SetCharacters(_characterScenarioContext);
        }

        public Transform GetTransform()
        {
            return _enemyView.GetTransform();
        }

        public bool GetActiveStatus()
        {
            return IsActive;
        }

        public void UseEffectsOnStart()
        {
            _effector.ProcessEffectsOnStart(_modifiableStats);
            Debug.LogWarning("Effects On Start Was Processed:");
        }

        public void UseEffectsOnEnd()
        {
            _effector.ProcessEffectsOnEnd(_modifiableStats);
            Debug.LogWarning("Effects On End Was Processed!");
        }

        public void AnalizeCondition()
        {
            Debug.Log("<color=#9C5F62>" + "--|Analyzing condition|-- " + "</color>");
            _analyzer.Analyze(_modifiableStats);
        }

        public void SwitchState(TypeOfConditionState state)
        {
            IConditionState newState = _stateFactory.CreateConditionState(state, this);

            if (newState != _conditionState)
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
    }
}