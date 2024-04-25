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
using System.Drawing;
using Unity.VisualScripting;

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
        public NavMeshObstacle NavMeshObstacle { get; set; }
        private CharacterUIView _UIView;
        public IInteractionProcessor InteractionProcessor { get; set; }
        public IInteractionDealer InteractionDealer { get; set; }
        public IInteractionCalculator InteractionCalculator { get; set; }
        public IDefaultBehaviourTree DefaultBehaviourTree { get; set; }
        public IEffectProcessor Effector { get; set; }
        public IAnalyzer Analyzer { get; set; }

        private Transform _slingShotInitPosition;
        private CharacterPooler _pooler;
        private CharacterUIPooler _characterUIPooler;
        private DiContainer _container;
        private NavMeshObstacle _navMeshObstacle;
        private CharacterUIViewmodel _uIViewmodel;
        private IConditionState _currentState;
        private IStateFactory _stateFactory;
        private ICharacterUIFactory _characterUIFactory;
        private ICharacterScenarioContext _characterScenarioContext;
        
        [Inject]
        public void Construct(
            SlingshotPooler slingShotPooler,   
            CharacterUIPooler characterUIPooler,
            IInteractionProcessor interactionProcessor,
            IInteractionDealer interactionDealer,
            IEffectProcessor effector,
            IInteractionCalculator interactionFinalizer,
            IStateFactory stateFactory,
            ICharacterUIFactory characterUIFactory,
            DiContainer container)
        {
            SlingShotPooler = slingShotPooler;
            _characterUIPooler = characterUIPooler;
            InteractionProcessor = interactionProcessor;
            InteractionDealer = interactionDealer;
            Effector = effector;
            InteractionCalculator = interactionFinalizer; 
            _stateFactory = stateFactory; 
            _characterUIFactory = characterUIFactory;
            _container = container;
        }

        public void Init(
        CharacterModel playerModel,
        CharacterView characterView,
        CharacterPooler characterPooler,
        CharacterUIView characterUIView)
        {
            DefaultBehaviourTree = _container.Resolve<IDefaultBehaviourTree>();
            DefaultBehaviourTree.InitTree(this);

            CharacterModel = playerModel;

            var stats = CharacterModel.GetStats();
            ModifiableStats = stats.ToReactive();

            CharacterView = characterView;
            CharacterView.Init(this);

            _currentState = _stateFactory.CreateConditionState(TypeOfConditionState.InactiveState, this);
            Analyzer = new Analyzer(this);

            _uIViewmodel = _characterUIFactory.CreateViewModel(ModifiableStats);

            _UIView = characterUIView;
            _pooler = characterPooler;
            
            _UIView.Init(CharacterView, _uIViewmodel);
            NavMeshAgent = CharacterView.NavMeshAgent;
            NavMeshAgent.enabled = false;
            _navMeshObstacle = CharacterView.NavMeshObstacle;
            _navMeshObstacle.carving = true;
            _navMeshObstacle.carveOnlyStationary = true;

            CharacterView.ON_CLICK += OnClick;
            CharacterView.ON_BEGINDRAG += OnBeginDrag;
            ON_STOP_MOVEMENT += CheckForEndOfState;
            SlingShotPooler.Init();
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
            if (!IsMoving)
            {
                _currentState.UseSlingshot(eventData, _slingShotInitPosition);
            }
        }

        public void Launch(Vector2 direction)
        {
            _currentState.LaunchYourself(direction);
        }

        public IInteraction GetInteraction()
        {
            return _currentState.GetInteraction(InteractionType.Knight_HeavyAttack);
        }

        public void ApplyInteraction(IInteraction interaction)
        {
            _currentState.ApplyInteraction(interaction);

            _uIViewmodel.UpdateStats(ModifiableStats);
        }

        public void ApplyBump(IInteractible interactible, IMovable bumpFromDealer)
        {
            if(IsMoving)
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
            ON_CHARACTER_DEATH(this);
            _pooler.Push(CharacterModel.Type, CharacterView);
        }

        public ReactiveStats GetCharacterStats()
        {
            return ModifiableStats;
        }

        public void AddEffects(List<IEffect> effects)
        {
            _currentState.AddEffects(effects);
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
        }

        public void ActivateUI()
        {
            _UIView.gameObject.SetActive(true);
        }

        public void DisableUI()
        {
            _UIView.gameObject.SetActive(false);
        }
    }
}
