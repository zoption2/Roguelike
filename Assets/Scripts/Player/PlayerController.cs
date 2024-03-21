using CharactersStats;
using Interactions;
using Prefab;
using SlingShotLogic;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Player
{
    public interface IPlayerController : ICharacterController
    {
        public void OnClick(Transform point, PointerEventData eventData);
        public void OnBeginDrag(PointerEventData eventData);
    }

    public delegate void OnEndTurn();
    public class PlayerController : IPlayerController, IDisposable
    {
        public event OnEndTurn ON_END_TURN;
        public bool IsActive { get; set; }

        private CharacterView _playerView;
        private CharacterModel _playerModel;
        private SlingshotPooler _slingShotPooler;
        private ISlingShot _slingShot;
        private List<IDisposable> _disposables;
        private Transform _SlingShotInitPosition;
        private Stats _playerStats;

        private IEffector _effector;
        private IInteractionProcessor _interactionProcessor;

        [Inject]
        public void Construct(
            SlingshotPooler slingShotPooler,
            IInteractionProcessor interactionProcessor,
            IEffector effector)
        {
            _slingShotPooler = slingShotPooler;
            _interactionProcessor = interactionProcessor;
            _effector = effector;
        }

        public void Init(
        CharacterModel playerModel,
        CharacterView playerView)
        {
            _playerModel = playerModel;
            _playerView = playerView;
            _playerView.Init(_playerModel);
            _playerStats = _playerModel.GetStats();
            _playerModel.Velocity.ToDisposableList(_disposables).Subscribe(EndTurn);
            _playerView.ON_CLICK += OnClick;
            _playerView.ON_BEGINDRAG += OnBeginDrag;
            _slingShotPooler.Init();
            _playerView.ON_COLLISION += ApplyInteractions;
            _playerView.ON_INTERACTION_FINISH += ApplyStats;
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
            float launchPower = _playerStats.LaunchPower;
            Vector2 forceVector = direction * launchPower;
            _playerView.AddImpulse(forceVector);

            _interactionProcessor.UseInteractionProcessor(_playerStats, _effector);
            _interactionProcessor.GetInteraction(InteractionType.BasicAttack);

            ///////INTERACTION////////
            //_interactionDealer.StartInteractionProcess(InteractionType.Knight_HeavyAttack);
            //_interactions = _interactionDealer.GetQueue();
            //////////////////////////

            _slingShot.OnShoot -= Launch;
        }

        public void ApplyInteractions(IInteractible interactible)
        {
            _interactionProcessor.GetInteractionHandler(interactible);
            _interactionProcessor.HandleInteraction();
        }

        public void ApplyStats(Stats updatetInteractionHandlerStats)
        {
            Debug.LogWarning($"Handler Stats before interaction. Healt: {_playerStats.Health}");
            _playerModel.ReactiveHealth.Value = updatetInteractionHandlerStats.Health;
            _playerStats.Health = _playerModel.ReactiveHealth.Value;
            Debug.LogWarning($"Handler Stats after interaction. Healt: {_playerStats.Health}");
        }


        public void EndTurn(float velocity)
        {
            if (Mathf.Abs(velocity) < 0.2f && velocity != 0)
            {
                ON_END_TURN?.Invoke();
                _playerView.IsPlayerMoving = false;
                /////////////////////////
                //_interactions.Clear();/
                /////////////////////////
            }
        }

        public void Dispose()
        {
            _playerView.ON_CLICK -= OnClick;
            _playerView.ON_BEGINDRAG -= OnBeginDrag;
            _playerView.ON_COLLISION -= ApplyInteractions;

            _slingShot.OnDirectionChange -= _playerView.ChangeDirection;
            _slingShot.OnShoot -= Launch;
            _playerView.ON_INTERACTION_FINISH -= ApplyStats;
        }
    }
}
