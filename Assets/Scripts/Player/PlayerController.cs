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

        private Queue<IInteraction> _interactions;

        private CharacterView _playerView;
        private CharacterModel _playerModel;
        private SlingshotPooler _slingShotPooler;
        private ISlingShot _slingShot;
        private List<IDisposable> _disposables;
        private Transform _SlingShotInitPosition;
        private CharacterPooler _pooler;

        private IEffector _effector;
        private IInteractionProcessor _interactionProcessor;
        private IInteractionDealer _interactionDealer;

        [Inject]
        public void Construct(
            SlingshotPooler slingShotPooler,      
            IInteractionProcessor interactionProcessor,
            IInteractionDealer interactionDealer,
            IEffector effector)
        {
            _slingShotPooler = slingShotPooler;
            _interactionProcessor = interactionProcessor;
            _interactionDealer = interactionDealer;
            _effector = effector;
        }

        public void Init(
        CharacterModel playerModel,
        CharacterView playerView,
        CharacterPooler characterPooler)
        {
            _playerModel = playerModel;
            _playerView = playerView;
            _pooler = characterPooler;
            _playerView.Init(this);
            _playerModel.Velocity.ToDisposableList(_disposables).Subscribe(EndTurn);
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
            float launchPower = _playerModel.LaunchPower;
            Vector2 forceVector = direction * launchPower;
            _playerView.AddImpulse(forceVector);

            _interactionProcessor.Init(_playerModel, _effector);

            //_interactionProcessor.GetInteraction(InteractionType.BasicAttack);

            ///////INTERACTION////////
            _interactionDealer.Init(_playerModel);
            _interactionDealer.UseInteraction(InteractionType.BasicAttack);
            _interactions = _interactionDealer.GetQueue();
            //////////////////////////

            _slingShot.OnShoot -= Launch;
        }

        public Queue<IInteraction> GetInteractions()
        { 
            return _interactionDealer.GetQueue(); 
        }    

        public void ApplyInteractions(Queue<IInteraction> interactions)
        {
            _interactionProcessor.HandleInteraction(interactions);
        }

        public void ApplyStats(CharacterModel updatetInteractionHandlerStats)
        {
            Debug.LogWarning($"Handler Stats before interaction. Healt: {_playerModel.Health}");
            _playerModel.ReactiveHealth.Value = updatetInteractionHandlerStats.Health;
            _playerModel.Health = _playerModel.ReactiveHealth.Value;
            Debug.LogWarning($"Handler Stats after interaction. Healt: {_playerModel.Health}");
        }


        public void EndTurn(float velocity)
        {
            if (Mathf.Abs(velocity) < 0.2f && velocity != 0)
            {
                ON_END_TURN?.Invoke();
                _playerView.IsPlayerMoving = false;
            }
        }

        public void Dispose()
        {
            _playerView.ON_CLICK -= OnClick;
            _playerView.ON_BEGINDRAG -= OnBeginDrag;

            _slingShot.OnDirectionChange -= _playerView.ChangeDirection;
            _slingShot.OnShoot -= Launch;
        }

        public CharacterModel GetCharacterModel()
        {
            return _playerModel;
        }
    }
}
