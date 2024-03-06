using CharactersStats;
using Pool;
using Prefab;
using SlingShotLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Player
{

    public interface IPlayerController : ICharacterController
    {
        public void OnClick(Transform point, PointerEventData eventData);
        public void OnBeginDrag(PointerEventData eventData);
        public void Init(PlayerModel model, ICharacterView playerView);
    }

    public delegate void OnEndTurn();
    public class PlayerController : IPlayerController, IDisposable
    {
        public event OnEndTurn OnEndTurn;

        private ICharacterView _playerView;
        private PlayerModel _playerModel;
        private SlingshotPooler _slingShotPooler;
        private ISlingShot _slingShot;
        private List<IDisposable> _disposables;
        private Transform _SlingShotInitPosition;
        public bool IsActive { get; set; }

        [Inject]
        public void Construct(
            SlingshotPooler slingShotPooler)
        {
            _slingShotPooler = slingShotPooler;
        }

        public void Init(
        PlayerModel playerModel,
        ICharacterView playerView)
        {
            _playerModel = playerModel;
            _playerView = playerView;
            _playerView.Init(_playerModel);
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
            if (IsActive && !_playerView.IsMoving)
            {
                PlayerType type = _playerModel.GetModelType();

                _slingShot = _slingShotPooler.Pull<ISlingShot>(type, _SlingShotInitPosition.position, _SlingShotInitPosition.rotation, _SlingShotInitPosition.parent);

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

        public void Launch(Vector2 direction, float dragDistance)
        {
            float launchPower = _playerModel.GetStats().LaunchPower;
            Vector2 forceVector = direction * dragDistance * launchPower;
            _playerView.AddImpulse(forceVector);
            _slingShot.OnShoot -= Launch;
        }

        public void EndTurn(float velocity)
        {
            if (velocity < 0.1f)
            {
                OnEndTurn?.Invoke();
                _playerView.IsMoving = false;
            }
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
