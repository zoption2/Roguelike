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
        public event OnEndTurn ON_END_TURN;
        public bool IsActive { get; set; }

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

        private IEffectProcessor _effector;
        private IInteractionProcessor _interactionProcessor;
        private IInteractionDealer _interactionDealer;
        private IInteractionFinalizer _interactionFinalizer;

        [Inject]
        public void Construct(
            SlingshotPooler slingShotPooler,      
            IInteractionProcessor interactionProcessor,
            IInteractionDealer interactionDealer,
            IEffectProcessor effector,
            IInteractionFinalizer interactionFinalizer)
        {
            _slingShotPooler = slingShotPooler;
            _interactionProcessor = interactionProcessor;
            _interactionDealer = interactionDealer;
            _effector = effector;
            _interactionFinalizer = interactionFinalizer;   
        }

        public void Init(
        CharacterModel playerModel,
        CharacterView playerView,
        CharacterPooler characterPooler)
        {
            _playerModel = playerModel;
            var stats = _playerModel.GetStats();
            _modifiableStats = stats.ToReactive();

            _interactionProcessor.Init(_effector);

            _playerView = playerView;
            _pooler = characterPooler;
            _playerView.Init(this);

            _modifiableStats.Velocity.ToDisposableList(_disposables).Subscribe(EndTurn);

            _playerView.ON_CLICK += OnClick;
            _playerView.ON_BEGINDRAG += OnBeginDrag;
            _slingShotPooler.Init();
        }

        public void OnClick(Transform point, PointerEventData eventData)
        {
            _SlingShotInitPosition = point;

            //////////////////////|Check effects on start|\\\\\\\\\\\\\\\\\\\\\
            _effector.ProcessEffectsOnStart(_modifiableStats);
            //////////////////////|----------------------|\\\\\\\\\\\\\\\\\\\\\

            Debug.LogWarning("Effects before interaction: \n");
            _effector.PrintEffects(_effector.GetPreInteractionEffects());

            Debug.LogWarning("Effects on Start interaction: \n");
            _effector.PrintEffects(_effector.GetOnStartTurnInteractionEffects());

            Debug.LogWarning("Effects on End interaction: \n");
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
            
            //////////////////////|Check effects before interaction|\\\\\\\\\\\\\\\\\\\\\
            _effector.ProcessStatsBeforeInteraction(_modifiableStats);
            //////////////////////|--------------------------------|\\\\\\\\\\\\\\\\\\\\\

            _interactionDealer.Init(_modifiableStats);
            IInteraction interaction = _interactionDealer.UseInteraction(InteractionType.BasicAttack);
            return interaction; 
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

        public void EndTurn(float velocity)
        {
            if (Mathf.Abs(velocity) < 0.2f && velocity != 0)
            {
                ON_END_TURN?.Invoke();
                _playerView.IsMoving = false;
                Debug.LogWarning("Hp Before Interaction: " + _modifiableStats.Health.Value);
                if (_interactionResult != null)
                {
                    
                    _modifiableStats = _interactionFinalizer.FinalizeInteraction(_modifiableStats, _interactionResult);
                }
                PushIfDead();
            }
        }

        public void PushIfDead()
        {
            Debug.LogWarning("Hp After Interaction: " + _modifiableStats.Health.Value);
            if (_modifiableStats.Health.Value <= 0)
            {
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

        public void SetCharacterContext(ICharacterScenarioContext characterScenarioContext)
        {
            _characterScenarioContext = characterScenarioContext;
        }

        public Transform GetTransform()
        {
            return _playerView.GetTransform();
        }

    }
}
