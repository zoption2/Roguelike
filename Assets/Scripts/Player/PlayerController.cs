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
        public event OnEndTurn OnEndTurn;

        public bool IsActive { get; set; }

        private CharacterView _playerView;
        private CharacterModel _playerModel;
        private SlingshotPooler _slingShotPooler;
        private ISlingShot _slingShot;
        private List<IDisposable> _disposables;
        private Transform _SlingShotInitPosition;

        private IInteractionDealer _interactionDealer;
        private Queue<IInteraction> _interactions;
        private CollidingObject _collidingObject;

        [Inject]
        public void Construct(
            SlingshotPooler slingShotPooler,
            IInteractionDealer interactionDealer)
        {
            _slingShotPooler = slingShotPooler;
            _interactionDealer = interactionDealer;
        }

        public void Init(
        CharacterModel playerModel,
        CharacterView playerView)
        {
            _playerModel = playerModel;
            _playerView = playerView;
            _playerView.Init(_playerModel);
            _playerModel.Velocity.ToDisposableList(_disposables).Subscribe(EndTurn);
            _playerView.ON_CLICK += OnClick;
            _playerView.ON_BEGINDRAG += OnBeginDrag;
            _slingShotPooler.Init();
            _collidingObject = _playerView.GetComponentInChildren<CollidingObject>();
            _playerView.ON_COLLISION += ApplyInteractions;

        }

        public void OnClick(Transform point, PointerEventData eventData)
        {
            _SlingShotInitPosition = point;
            _interactionDealer.Init(_playerModel.GetStats());
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsActive && !_playerView.IsPlayerMoving)
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
        }

        public void Launch(Vector2 direction)
        {
            float launchPower = _playerModel.GetStats().LaunchPower;
            Vector2 forceVector = direction * launchPower;
            _playerView.AddImpulse(forceVector);

            ///////////////
            _interactionDealer.StartInteractionProcess(InteractionType.BasicAttack);
            _interactions = _interactionDealer.GetQueue();

            _slingShot.OnShoot -= Launch;
        }

        //public void ProcessInteractions(Queue<IInteraction> queue)
        //{
        //    Stats _interationHandlerStatsCopy = _playerModel.GetStats();
        //    foreach (var interaction in queue)
        //    {
        //        Debug.LogWarning($"HP before attack: {_playerModel.Health}");
        //        interaction.Interacte(_interationHandlerStatsCopy);
        //        Debug.LogWarning($"Attack type: {interaction.GetType()}, HP after attack: {_interationHandlerStatsCopy.Health}");
        //        queue.Dequeue();
        //    }
        //}

        public void ApplyInteractions(IInteractible interactible)
        {
            //Stats _interationHandlerStatsCopy = _playerModel.GetStats();
            //Queue<IInteraction> interactions = _interactions;
            interactible.ProcessInteractions(_interactions);
            
        }


        public void EndTurn(float velocity)
        {
            if (Mathf.Abs(velocity) < 0.2f && velocity != 0)
            {
                OnEndTurn?.Invoke();
                _playerView.IsPlayerMoving = false;
            }
        }

        public void Dispose()
        {
            _playerView.ON_CLICK -= OnClick;
            _playerView.ON_BEGINDRAG -= OnBeginDrag;
            _playerView.ON_COLLISION -= ApplyInteractions;

            _slingShot.OnDirectionChange -= _playerView.ChangeDirection;

            _slingShot.OnShoot -= Launch;
        }
    }
}
