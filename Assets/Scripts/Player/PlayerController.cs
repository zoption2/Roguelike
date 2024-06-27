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
using System.Linq;
using Enemy;
using Abilities;
using Projectiles;


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

        public bool IsDead { get; set; }
        public IAbility CurrentAbility { get; set; }
        public IInteractionProcessor InteractionProcessor { get; set; }
        public IInteractionDealer InteractionDealer { get; set; }
        public IInteractionCalculator InteractionCalculator { get; set; }
        public IDefaultBehaviourTree DefaultBehaviourTree { get; set; }
        public IEffectProcessor Effector { get; set; }
        public IAnalyzer Analyzer { get; set; }
        public CharacterView CharacterView { get; set; }
        public CharacterModel CharacterModel { get; set; }
        public SlingshotPooler SlingShotPooler { get; set; }
        public ReactiveStats ModifiableStats { get; set; }
        public NavMeshAgent NavMeshAgent { get; set; }
        public NavMeshObstacle NavMeshObstacle { get; set; }
        public List<IProjectile> LaunchedProjectiles { get; set; }

        private CharacterUIView _UIView;
        private Transform _slingShotInitPosition;
        private CharacterPooler _characterPooler;
        private CharacterUIPooler _characterUIPooler;
        private DiContainer _container;
        private NavMeshObstacle _navMeshObstacle;
        private CharacterUIViewmodel _uIViewmodel;
        private ReactiveList<IEffect> _allEffects;
        private List<IAbility> _abilitiesForReload;
        private IAbility _basicAbility;
        private IConditionState _currentState;
        private IStateFactory _stateFactory;
        private IRoomContext _characterScenarioContext;
        private IUIFactory _uIFactory;
        

        [Inject]
        public void Construct(
            IPoolManager poolManager,
            IInteractionProcessor interactionProcessor,
            IInteractionDealer interactionDealer,
            IEffectProcessor effector,
            IInteractionCalculator interactionFinalizer,
            IStateFactory stateFactory,
            IUIFactory uIFactory,
            DiContainer container)
        {
            SlingShotPooler = poolManager.GetSlingshotPooler();
            _characterPooler = poolManager.GetCharacterPooler();
            _characterUIPooler = poolManager.GetCharacterUIPooler();
            InteractionProcessor = interactionProcessor;
            InteractionDealer = interactionDealer;
            Effector = effector;
            InteractionCalculator = interactionFinalizer; 
            _stateFactory = stateFactory; 
            _uIFactory = uIFactory; 
            _container = container;
        }

        public void Init(
        CharacterModel playerModel,
        CharacterView characterView,
        CharacterUIView characterUIView)
        {
            DefaultBehaviourTree = _container.Resolve<IDefaultBehaviourTree>();
            DefaultBehaviourTree.InitTree(this);
            DefaultBehaviourTree.SetAbilities(playerModel.Abilities);

            CharacterModel = playerModel;

            var stats = CharacterModel.GetStats();
            ModifiableStats = stats.ToReactive();

            _allEffects = CharacterModel.GetAllEffects();

            CharacterView = characterView;
            CharacterView.Init(this);

            _currentState = _stateFactory.CreateConditionState(TypeOfConditionState.InactiveState, this);
            Analyzer = new Analyzer(this);

            _UIView = characterUIView;       

            _uIViewmodel = new CharacterUIViewmodel();
            _uIViewmodel.Init(CharacterModel, _uIFactory, _UIView, this);

            _UIView.Init(CharacterView, _uIViewmodel);
            Effector.Init(_uIViewmodel, _allEffects);

            NavMeshAgent = CharacterView.NavMeshAgent;
            NavMeshAgent.enabled = false;
            _navMeshObstacle = CharacterView.NavMeshObstacle;
            _navMeshObstacle.carving = true;
            _navMeshObstacle.carveOnlyStationary = true;

            CharacterView.ON_CLICK += OnClick;
            CharacterView.ON_BEGINDRAG += OnBeginDrag;
            ON_STOP_MOVEMENT += CheckForEndOfState;

            foreach(var ability in CharacterModel.Abilities)
            {
                if(ability is BasicAttackAbility)
                {
                    _basicAbility = ability;
                }
            }

            CurrentAbility = _basicAbility;
            LaunchedProjectiles = new List<IProjectile>();
        }

        public void DoUpdate()
        {
            _currentState.DoUpdate();
        }


        public void SetCurrentAbility(IAbility ability)
        {
            CurrentAbility = ability;
            Debug.LogWarning(CurrentAbility + "  " + CurrentAbility.ProjectileType);
        }


        public void OnClick(Transform point, PointerEventData eventData)
        {
            _slingShotInitPosition = point;
            if(IsActive)
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
            _uIViewmodel.DeactivateSkillsBTNs();
            if (!IsMoving && LaunchedProjectiles.Count == 0)
            {
                _currentState.UseSlingshot(eventData, _slingShotInitPosition);
            }
        }

        public IInteraction GetInteraction()
        {
            return _currentState.GetInteraction();
        }

        public void ApplyInteraction(IInteraction interaction)
        {
            _currentState.ApplyInteraction(interaction);

            _uIViewmodel.UpdateStats(ModifiableStats);
            _uIViewmodel.VisualiseEffects(_allEffects.Value);
        }

        public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer)
        {
            if (interactible is IProjectile)
            {
                _currentState.ApplyBump(interactible, bumpFromDealer);
            }
            else if (IsActive)
            {
                _currentState.ApplyBump(interactible, bumpFromDealer);
            }
        }

        public void CheckForEndOfState()
        {
            _characterScenarioContext.CheckIfAllStopped();
        }

        public void PushIfDead()
        {
            Debug.Log("pushed player to pool!");
            _characterPooler.Push(CharacterModel.Type, CharacterView);
            PushCharacterUI();
            ON_CHARACTER_DEATH?.Invoke(this);
        }

        public void JustPush()
        {
            //_characterPooler.Push(CharacterModel.Type, CharacterView);
            PushCharacterUI();
            ON_CHARACTER_DEATH = null;

        }

        public ReactiveStats GetCharacterStats()
        {
            return ModifiableStats;
        }

        public void AddEffects(List<IEffect> effects)
        {
            _currentState.AddEffects(effects);
            _uIViewmodel.VisualiseEffects(_allEffects.Value);
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
        }

        public bool CheckIfMoving()
        {
            return IsMoving;
        }
        public void SetCharacterContext(IRoomContext characterScenarioContext)
        {
            _characterScenarioContext = characterScenarioContext;
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
            //Debug.LogWarning("Effects On Start Was Processed!");
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

            if (_currentState.GetType() != newState.GetType())
            {
                _currentState?.OnExit();
                _currentState = newState;
                _currentState.OnEnter();

                if (state == TypeOfConditionState.DeadState)
                {
                    IsDead = true;
                }
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

        public void Dispose()
        {
            CharacterView.ON_CLICK -= OnClick;
            CharacterView.ON_BEGINDRAG -= OnBeginDrag;
            ON_STOP_MOVEMENT -= CheckForEndOfState;
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
            CurrentAbility.UseAbility();
            if (!CurrentAbility.ReadyForUse)
            {
                _uIViewmodel.ChangeButtonInteractible(CurrentAbility,false);
            }
        }

        public void ProcessOnStartTurn()
        {
            _uIViewmodel.ToggleActiveIndicator();
            _abilitiesForReload = CharacterModel.Abilities.Where(x => !x.ReadyForUse).ToList();
            foreach (IAbility ability in CharacterModel.Abilities)
            {
                ability.TickReload();
            }
            _uIViewmodel.UpdateReloadIndicators();
            CurrentAbility = _basicAbility;
        }

        public void RevertReadyUnactiveAbilityButtons()
        {
            foreach (IAbility ability in CharacterModel.Abilities)
            {
                if (ability.ReadyForUse)
                {
                    _uIViewmodel.ChangeButtonInteractible(ability, true);
                }
            }
        }
        public void ProcessOnEndTurn()
        {
            _uIViewmodel.ToggleActiveIndicator();
            
            if (_abilitiesForReload.Count > 0)
            {
                foreach (IAbility ability in _abilitiesForReload)
                {
                    if (ability.ReadyForUse)
                    {
                        _uIViewmodel.ChangeButtonInteractible(ability,true);
                    }
                }
            }


            foreach(IEnemyController enemy in _characterScenarioContext.Enemies)
            {
                enemy.DisactivateAbilityPanel();
            }
            RevertReadyUnactiveAbilityButtons();
        }

        
    }
}
