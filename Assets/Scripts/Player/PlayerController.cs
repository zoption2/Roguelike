using CharactersStats;
using Interactions;
using Gameplay;
using Pool;
using Prefab;
using SlingShotLogic;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Zenject.SpaceFighter;
using Obstacles;

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
        //public event OnEndTurn ON_END_TURN;
        public bool IsActive { get; set; }

        public event OnCharacterDeath On_Character_Death;

        private ICharacterScenarioContext _characterScenarioContext;
        private CharacterView _playerView;
        private CharacterModel _playerModel;
        private SlingshotPooler _slingShotPooler;
        private ISlingShot _slingShot;
        private List<IDisposable> _disposables;
        private Transform _SlingShotInitPosition;
        private CharacterPooler _pooler;
        private ReactiveStats _modifiableStats;
        private ReactiveStats _interactionResult;

        private IAnalyzer _analyzer;
        private IConditionState _conditionState;
        private IStateFactory _stateFactory;

        private IEffectProcessor _effector;
        private IInteractionProcessor _interactionProcessor;
        private IInteractionDealer _interactionDealer;
        private IInteractionCalculator _interactionFinalizer;

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

            //_modifiableStats.Velocity.ToDisposableList(_disposables).Subscribe(EndTurn);

            _playerView.ON_CLICK += OnClick;
            _playerView.ON_BEGINDRAG += OnBeginDrag;
            _playerView.On_Stop_Movement += CheckForEndOfState;
            _slingShotPooler.Init();
        }

        public void OnClick(Transform point, PointerEventData eventData)
        {
            _SlingShotInitPosition = point;

            Debug.LogWarning("Effects before interaction:");
            _effector.PrintEffects(_effector.GetPreInteractionEffects());

            Debug.LogWarning("Effects on Start interaction:");
            _effector.PrintEffects(_effector.GetOnStartTurnInteractionEffects());

            Debug.LogWarning("Effects on End interaction:");
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
            _playerView.AddImpulse(forceVector);

            _slingShot.OnShoot -= Launch;
        }

        public IInteraction GetInteraction()
        {
            if(IsActive)
            {
                //////////////////////|Check effects before interaction|\\\\\\\\\\\\\\\\\\\\\
                ReactiveStats statsWithBonus = _effector.ProcessStatsBeforeInteraction(_modifiableStats);
                //////////////////////|--------------------------------|\\\\\\\\\\\\\\\\\\\\\

                _interactionDealer.Init(statsWithBonus);
                IInteraction interaction = _interactionDealer.UseInteraction(InteractionType.BasicAttack);
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
            }
        }

        public void CheckForEndOfState()
        {
            _characterScenarioContext.CheckIfAllStopped();
            EndInteraction();
        }

        public void EndInteraction()
        {
            //Debug.Log("<color=#189C0C>" + "Hp On Start Turn: " + _modifiableStats.Health.Value + "</color>");

            if (_interactionResult != null)
            {

                _modifiableStats = _interactionFinalizer.FinalizeInteraction(_modifiableStats, _interactionResult);
                _interactionResult = null;
            }
            PushIfDead();
        }

        public void PushIfDead()
        {
            //Debug.Log("<color=#9C3C15>" + "Hp On End Turn: " + _modifiableStats.Health.Value + "</color>");

            if (_modifiableStats.Health.Value <= 0)
            {
                On_Character_Death(this);
                _pooler.Push(_playerModel.Type, _playerView);
            }
        }

        public void Dispose()
        {
            _playerView.ON_CLICK -= OnClick;
            _playerView.ON_BEGINDRAG -= OnBeginDrag;

            _slingShot.OnDirectionChange -= _playerView.ChangeDirection;
            _slingShot.OnShoot -= Launch;
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
            Debug.Log("Enemy has attacked");
        }

        public void Move()
        {
            Debug.Log("Enemy has moved");
        }

        public void Tick()
        {

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
            return _playerModel.Type;
        }
    }
}
