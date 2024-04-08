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
using Zenject.SpaceFighter;
using System.Threading.Tasks;

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

        private CharacterView _playerView;
        private CharacterModel _playerModel;
        private SlingshotPooler _slingShotPooler;
        private Transform _SlingShotInitPosition;
        private CharacterPooler _pooler;
        private ReactiveStats _modifiableStats;
        private ReactiveStats _interactionResult;
        private ISlingShot _slingShot;
        private NavMeshAgent _navMeshAgent;
        private IAnalyzer _analyzer;
        private IConditionState _conditionState;
        private IStateFactory _stateFactory;
        private ICharacterScenarioContext _characterScenarioContext;
        private IEffectProcessor _effector;
        private IInteractionProcessor _interactionProcessor;
        private IInteractionDealer _interactionDealer;
        private IInteractionCalculator _interactionFinalizer;
        private int _milisecondsDelay = 3000;

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
            _interactionProcessor = interactionProcessor;
            _interactionDealer = interactionDealer;
            _effector = effector;
            _interactionFinalizer = interactionFinalizer; 
            _stateFactory = stateFactory;   
        }

        public void Init(
        CharacterModel playerModel,
        CharacterView playerView,
        CharacterPooler characterPooler)
        {
            _playerModel = playerModel;

            var stats = _playerModel.GetStats();
            _modifiableStats = stats.ToReactive();

            _conditionState = _stateFactory.CreateConditionState(TypeOfConditionState.DefaultState, this);
            _analyzer = new Analyzer(this);

            _playerView = playerView;
            _pooler = characterPooler;
            _playerView.Init(this);
            _navMeshAgent = _playerView.NavMeshAgent;
            _navMeshAgent.enabled = false;

            _playerView.ON_CLICK += OnClick;
            _playerView.ON_BEGINDRAG += OnBeginDrag;
            _playerView.ON_STOP_MOVEMENT += CheckForEndOfState;
            _slingShotPooler.Init();
        }

        public void OnClick(Transform point, PointerEventData eventData)
        {
            _SlingShotInitPosition = point;

            Debug.Log($"-----|{_playerModel.Type}|-----");
            Debug.Log("<color=#189C0C>" + "Hp: " + _modifiableStats.Health.Value + "</color>");

            Debug.Log("<color=#F4DA64>" + "Effects Before interaction: " + "</color>");
            _effector.PrintEffects(_effector.GetPreInteractionEffects());

            Debug.Log("<color=#F4DA64>" + "Effects on Start interaction: " + "</color>");
            _effector.PrintEffects(_effector.GetOnStartTurnInteractionEffects());

            Debug.Log("<color=#F4DA64>" + "Effects on End interaction: " + "</color>");
            _effector.PrintEffects(_effector.GetOnEndTurnInteractionEffects());
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsActive && !_playerView.IsMoving)
            {
                UseSlingshot(eventData);
            }
        }

        private void UseSlingshot(PointerEventData eventData)
        {
            CharacterType type = _playerModel.Type;

            Vector3 fixedInitPosition = new Vector3(_SlingShotInitPosition.position.x, _SlingShotInitPosition.position.y, _SlingShotInitPosition.position.z - 1f);

            _slingShot = _slingShotPooler.Pull<ISlingShot>(type, fixedInitPosition, Quaternion.identity, _SlingShotInitPosition.parent);

            _slingShot.Init(_SlingShotInitPosition.position, type);

            _slingShot.OnDirectionChange -= _playerView.ChangeDirection;
            _slingShot.OnDirectionChange += _playerView.ChangeDirection;

            _slingShot.OnShoot -= Launch;
            _slingShot.OnShoot += Launch;

            DragInputModule.dragFocusObject = _slingShot.gameObject;
            eventData.pointerDrag = _slingShot.gameObject;
            eventData.dragging = true;
        }

        public void Launch(Vector2 direction)
        {
            float launchPower = _modifiableStats.LaunchPower.Value;
            Vector2 forceVector = direction * launchPower;
            _playerView.Rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
            _slingShot.OnShoot -= Launch;
        }

        public IInteraction GetInteraction()
        {
            if(IsActive)
            {
                ReactiveStats statsWithBonus = _effector.ProcessStatsBeforeInteraction(_modifiableStats);

                _interactionDealer.Init(statsWithBonus);
                IInteraction interaction = _interactionDealer.UseInteraction(InteractionType.Knight_HeavyAttack);
                return interaction;
            } 
            else
            {
                IInteraction interaction = _interactionDealer.UseInteraction(InteractionType.None);
                return interaction;
            }
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

                AnalizeCondition();
            }
        }

        public void CheckForEndOfState()
        {
            _characterScenarioContext.CheckIfAllStopped();
        }

        public void PushIfDead()
        {
            ON_CHARACTER_DEATH(this);
            _pooler.Push(_playerModel.Type, _playerView);
        }

        public ReactiveStats GetCharacterStats()
        {
            return _modifiableStats;
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

        public async void SkipTurn()
        {
            await Task.Delay(_milisecondsDelay);
            _playerView.TrySkipTurn();
            
        }

        public bool CheckIfMoving()
        {
            return _playerView.IsMoving;
        }
        public void SetCharacterContext(ICharacterScenarioContext characterScenarioContext)
        {
            _characterScenarioContext = characterScenarioContext;
        }

        public Transform GetTransform()
        {
            return _playerView.GetTransform();
        }

        public bool GetActiveStatus()
        {
            return IsActive;
        }

        public void UseEffectsOnStart()
        {
            _effector.ProcessEffectsOnStart(_modifiableStats);
            Debug.LogWarning("Effects On Start Was Processed!");
        }

        public void UseEffectsOnEnd()
        {
            _effector.ProcessEffectsOnEnd(_modifiableStats);
            Debug.LogWarning("Effects On End Was Processed!");
        }

        public void AnalizeCondition()
        {
            Debug.Log("<color=#9C5F62>" + "--|Analyzing condition|-- " + "</color>");
            _analyzer.Analyze(_modifiableStats, _effector);
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
            return _playerModel.Type;
        }

        public void Dispose()
        {
            _playerView.ON_CLICK -= OnClick;
            _playerView.ON_BEGINDRAG -= OnBeginDrag;

            _slingShot.OnDirectionChange -= _playerView.ChangeDirection;
            _slingShot.OnShoot -= Launch;
        }
    }
}
