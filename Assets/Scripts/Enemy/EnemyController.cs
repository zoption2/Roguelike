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


namespace Enemy
{
    public interface IEnemyController : ICharacterController
    {
        public void OnClick(Transform point, PointerEventData eventData);
        public void DisactivateAbilityPanel();
    }
    public class EnemyController : IEnemyController, IControllerInputs, IDisposable
    {
        public event OnCharacterDeath ON_CHARACTER_DEATH;
        public event OnStopMovement ON_STOP_MOVEMENT;
        public bool IsActive { get; set; }
        public bool IsStunned { get; set; }
        public bool IsMoving { get; set; }
        public IAbility CurrentAbility { get; set; }
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
        public NavMeshObstacle NavMeshObstacle { get; set; }

        private IConditionState _currentState;
        private IStateFactory _stateFactory;
        private ICharacterScenarioContext _characterScenarioContext;
        private IUIFactory _uIFactory;
        private CharacterUIView _UIView;
        private ReactiveList<IEffect> _allEffects;
        private Transform _slingShotInitPosition;
        private CharacterPooler _pooler;
        private CharacterUIPooler _characterUIPooler;
        private CharacterUIViewmodel _uIViewmodel;
        private DiContainer _container;

        [Inject]
        public void Construct(
            CharacterUIPooler characterUIPooler,
            IInteractionProcessor interactionProcessor,
            IInteractionDealer interactionDealer,
            IEffectProcessor effector,
            IInteractionCalculator interactionFinalizer,
            IStateFactory stateFactory,
            IUIFactory uIFactory,
            DiContainer container)
        {
            _characterUIPooler = characterUIPooler;
            InteractionProcessor = interactionProcessor;
            InteractionDealer = interactionDealer;
            Effector = effector;
            InteractionCalculator = interactionFinalizer;
            _stateFactory = stateFactory;
            _uIFactory = uIFactory;
            _container = container;
        }

        public void Init(
            CharacterModel characterModel,
            CharacterView characterView,
            CharacterPooler characterPooler,
            CharacterUIView characterUIView)
        {
            DefaultBehaviourTree = _container.Resolve<IDefaultBehaviourTree>();
            DefaultBehaviourTree.InitTree(this);
            DefaultBehaviourTree.SetAbilities(characterModel.Abilities);

            CharacterModel = characterModel;

            var stats = CharacterModel.GetStats();
            ModifiableStats = stats.ToReactive();

            _allEffects = CharacterModel.GetAllEffects();

            CharacterView = characterView;
            CharacterView.Init(this);

            _currentState = _stateFactory.CreateConditionState(TypeOfConditionState.InactiveState, this);
            Analyzer = new Analyzer(this);

            InteractionDealer.Init(ModifiableStats);

            _UIView = characterUIView;
            _pooler = characterPooler;

            _uIViewmodel = new CharacterUIViewmodel();
            _uIViewmodel.Init(CharacterModel, _uIFactory, _UIView, this);

            _UIView.Init(CharacterView, _uIViewmodel);
            Effector.Init(_uIViewmodel, _allEffects);

            NavMeshAgent = CharacterView.NavMeshAgent;
            NavMeshAgent.updateUpAxis = false;
            NavMeshAgent.updateRotation = false;
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

        public void SetCurrentAbility(IAbility ability)
        {
        }

        public void DisactivateAbilityPanel()
        {
            _uIViewmodel.DeactivateSkillsBTNs();
        }


        public void OnClick(Transform point, PointerEventData eventData)
        {
            _slingShotInitPosition = point;
            bool isPlayerTurn = true;
            foreach(ICharacterController enemy in _characterScenarioContext.Enemies)
            {
                if(enemy.IsActive)
                {
                    isPlayerTurn = false;
                    break;
                }
            }
            if (isPlayerTurn)
            {
                _uIViewmodel.ActivateSkillsBTNs();
            }
        }

        public float GetCurrentLaunchDistance()
        {
            float launchPower = ModifiableStats.LaunchPower.Value;
            float dragConstant = GetRigidbody().drag;
            float maxDistance = launchPower / dragConstant;
            float currentLaunchDistance = maxDistance * CurrentAbility.GetLaunchModifier();
            return currentLaunchDistance;
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
            return _currentState.GetInteraction();
        }

        public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer)
        {
            if(interactible is IProjectile)
            {
                _currentState.ApplyBump(interactible, bumpFromDealer);
            }
            else if (IsActive)
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
            ON_STOP_MOVEMENT -= CheckForEndOfState;
        }

        public ReactiveStats GetCharacterStats()
        {
            return ModifiableStats;
        }

        public void ApplyInteraction(IInteraction interaction)
        {
            _currentState.ApplyInteraction(interaction);

            _uIViewmodel.UpdateStats(ModifiableStats);
            _uIViewmodel.VisualiseEffects(_allEffects.Value);
        }

        public void AddEffects(List<IEffect> effects)
        {
            _currentState.AddEffects(effects);
            _uIViewmodel.VisualiseEffects(_allEffects.Value);
        }
        public void CheckForEndOfState()
        {
            _characterScenarioContext.CheckIfAllStopped();
        }

        public void PushIfDead()
        {
            _pooler.Push(CharacterModel.Type, CharacterView);
            PushCharacterUI();
            ON_CHARACTER_DEATH?.Invoke(this);
        }

        public void JustPush()
        {
            _pooler.Push(CharacterModel.Type, CharacterView);
            PushCharacterUI();
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

        public void SetCharacterContext(ICharacterScenarioContext characterScenarioContext)
        {
            _characterScenarioContext = characterScenarioContext;
            DefaultBehaviourTree.SetCharacters(_characterScenarioContext);
        }

        public Transform GetTransform()
        {
            return CharacterView.GetTransform();
        }

        public Transform GetProjectileSpawn()
        {
            return CharacterView.GetProjectileSpawn();
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

        public Rigidbody GetRigidbody()
        {
            return CharacterView.GetRigidbody();
        }

        public Vector3 GetVelocity()
        {
            return CharacterView.GetVelocity();
        }

        public void PushCharacterUI()
        {
            _characterUIPooler.Push(UIType.CharacterUI, _UIView);
        }

        public void ActivateUI()
        {
            _UIView.gameObject.SetActive(true);
        }

        public void DisableUI()
        {
            _UIView.gameObject.SetActive(false);
        }

        public void UpdateEffectsOnUI(List<IEffect> displayedEffects)
        {
            _uIViewmodel.VisualiseEffects(displayedEffects);
        }

        public void UpdateHealthBar()
        {
            _uIViewmodel.UpdateHealthBar();
        }

        public void ProcessReloadAbility()
        {
        }

        public void ProcessOnStartTurn()
        {
            _uIViewmodel.ToggleActiveIndicator();
        }

        public void RevertReadyUnactiveAbilityButtons()
        {
            
        }

        public void ProcessOnEndTurn()
        {
            _uIViewmodel.ToggleActiveIndicator();
            
            foreach (IAbility ability in CharacterModel.Abilities)
            {
                ability.TickReload();
            }
            _uIViewmodel.UpdateReloadIndicators();
        }
    }
}