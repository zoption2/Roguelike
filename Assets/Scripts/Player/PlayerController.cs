using CharactersStats;
using Interactions;
using Gameplay;
using Pool;
using SlingShotLogic;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using UnityEngine.AI;

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
        public bool IsActive { get; set; }
        public bool IsStunned { get; set; }
        public event OnCharacterDeath ON_CHARACTER_DEATH;
        public event OnStopMovement ON_STOP_MOVEMENT;

        public CharacterView CharacterView { get; set; }
        private CharacterModel _playerModel;
        private SlingshotPooler _slingShotPooler;
        private Transform _slingShotInitPosition;
        private CharacterPooler _pooler;
        public ReactiveStats ModifiableStats { get; set; }
        private ReactiveStats _interactionResult;
        private NavMeshAgent _navMeshAgent;
        private ISlingShot _slingShot;
        public IAnalyzer Analyzer { get; set; }
        public bool IsMoving { get; set; }
        private IConditionState _conditionState;
        private IStateFactory _stateFactory;
        private ICharacterScenarioContext _characterScenarioContext;
        public IEffectProcessor Effector { get; set; }
        public IInteractionProcessor InteractionProcessor { get; set; }
        public IInteractionDealer InteractionDealer { get; set; }
        public IInteractionCalculator InteractionFinalizer { get; set; }

        [Inject]
        public void Construct(
            SlingshotPooler slingShotPooler,      
            IInteractionProcessor interactionProcessor,
            IInteractionDealer interactionDealer,
            IEffectProcessor effector,
            IInteractionCalculator interactionFinalizer,
            IStateFactory stateFactory)
        {
            _slingShotPooler = slingShotPooler;
            InteractionProcessor = interactionProcessor;
            InteractionDealer = interactionDealer;
            Effector = effector;
            InteractionFinalizer = interactionFinalizer; 
            _stateFactory = stateFactory;   
        }

        public void Init(
        CharacterModel playerModel,
        CharacterView playerView,
        CharacterPooler characterPooler)
        {
            _playerModel = playerModel;

            var stats = _playerModel.GetStats();
            ModifiableStats = stats.ToReactive();

            _conditionState = _stateFactory.CreateConditionState(TypeOfConditionState.InactiveState, this);
            Analyzer = new Analyzer(this);

            CharacterView = playerView;
            _pooler = characterPooler;
            CharacterView.Init(this);
            _navMeshAgent = CharacterView.NavMeshAgent;
            _navMeshAgent.enabled = false;

            CharacterView.ON_CLICK += OnClick;
            CharacterView.ON_BEGINDRAG += OnBeginDrag;
            ON_STOP_MOVEMENT += CheckForEndOfState;
            _slingShotPooler.Init();
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
            _slingShotInitPosition = point;

            Debug.Log($"-----|{_playerModel.Type}|-----");
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
            if (IsActive && !IsMoving)
            {
                UseSlingshot(eventData);
            }
        }

        private void UseSlingshot(PointerEventData eventData)
        {
            CharacterType type = _playerModel.Type;

            Vector3 fixedInitPosition = new Vector3(_slingShotInitPosition.position.x, _slingShotInitPosition.position.y, _slingShotInitPosition.position.z - 1f);

            _slingShot = _slingShotPooler.Pull<ISlingShot>(type, fixedInitPosition, Quaternion.identity, _slingShotInitPosition.parent);

            _slingShot.Init(_slingShotInitPosition.position, type);

            _slingShot.OnDirectionChange -= CharacterView.ChangeDirection;
            _slingShot.OnDirectionChange += CharacterView.ChangeDirection;

            _slingShot.OnShoot -= Launch;
            _slingShot.OnShoot += Launch;

            DragInputModule.dragFocusObject = _slingShot.gameObject;
            eventData.pointerDrag = _slingShot.gameObject;
            eventData.dragging = true;
        }

        public void Launch(Vector2 direction)
        {
            _conditionState.Launch(direction);
            _slingShot.OnShoot -= Launch;
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
            _pooler.Push(_playerModel.Type, CharacterView);
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
            Debug.Log("Player has attacked");
        }

        public void Move()
        {
            Debug.Log("Player has moved");
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
            return _playerModel.Type;
        }

        public void Dispose()
        {
            CharacterView.ON_CLICK -= OnClick;
            CharacterView.ON_BEGINDRAG -= OnBeginDrag;

            _slingShot.OnDirectionChange -= CharacterView.ChangeDirection;
            _slingShot.OnShoot -= Launch;
        }
        public IConditionState GetCurrentConditionState()
        {
            return _conditionState;
        }
    }
}
