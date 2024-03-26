using CharactersStats;
using Interactions;
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

        private IInteraction _interaction;

        private CharacterView _playerView;
        private CharacterModel _playerModel;
        private SlingshotPooler _slingShotPooler;
        private ISlingShot _slingShot;
        private List<IDisposable> _disposables;
        private Transform _SlingShotInitPosition;
        private CharacterPooler _pooler;
        private ModifiableStats _modifiableStats;

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

            _modifiableStats = new ModifiableStats(_playerModel.GetStats());
            _interactionProcessor.Init(_effector);
            _interactionFinalizer.Init(_interactionProcessor, _effector);

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
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsActive && !_playerView.IsPlayerMoving)
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

            _interactionDealer.Init(_modifiableStats);
            _interaction = _interactionDealer.UseInteraction(InteractionType.BasicAttack);

            _slingShot.OnShoot -= Launch;
        }

        public IInteraction GetInteraction()
        { 
            return _interaction; 
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
                        _effector.AddEffects(effect);
                    }
                }
                _interactionProcessor.ProcessInteraction(interaction);
            }
        }

        public void EndTurn(float velocity)
        {
            if (Mathf.Abs(velocity) < 0.2f && velocity != 0)
            {
                ON_END_TURN?.Invoke();
                _playerView.IsPlayerMoving = false;
                _modifiableStats = _interactionFinalizer.FinalizeInteraction(_modifiableStats);
            }
        }

        public void Dispose()
        {
            _playerView.ON_CLICK -= OnClick;
            _playerView.ON_BEGINDRAG -= OnBeginDrag;

            _slingShot.OnDirectionChange -= _playerView.ChangeDirection;
            _slingShot.OnShoot -= Launch;
        }

        public ModifiableStats GetCharacterStats()
        {
            return _modifiableStats;
        }

        public void AddEffects(List<IEffect> effects)
        {
            if (effects != null && effects.Count > 0)
            {
                foreach (IEffect effect in effects)
                {
                    _effector.AddEffects(effect);
                }
            }
            _effector.ProcessStatsBeforeInteraction(_modifiableStats);
        }
    }
}
