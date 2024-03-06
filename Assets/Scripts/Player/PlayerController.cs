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
        public void Init(PlayerModel model, CharacterView playerView);
    }

    public delegate void OnEndTurn();
    public class PlayerController : IPlayerController, IDisposable
    {
        private Transform _pointerPosition;
        private Vector2 _initPosition;

        private ICharacterView _playerView;
        private PlayerModel _playerModel;
        private PlayerType _type;
        private SlingshotPooler _slingShotPooler;

        private Rigidbody2D _playerViewRigidbody;

        private SlingShot _slingShot;

        private GameObject _slingShotObject;

        private float _maxPower = 50f;

        private bool _isLaunchWasSubscibed;

        private bool _isChangeDirectionSubscibed;

        private List<IDisposable> _disposables;

        public event OnEndTurn OnEndTurn;

        public bool IsActive { get; set; }

        [Inject]
        public void Construct(
            SlingshotPooler slingShotPooler)
        {
            _slingShotPooler = slingShotPooler;
        }

        public void Init(
        PlayerModel playerModel,
        CharacterView playerView)
        {
            _playerModel = playerModel;
            _playerView = playerView;
            _playerView.Init(_playerModel);
            _playerModel.Velocity.ToDisposableList(_disposables).Subscribe(EndTurn);
            _type = _playerModel.GetModelType();
            _playerView.ON_CLICK += OnClick;
            _playerView.ON_BEGINDRAG += OnBeginDrag;
            _slingShotPooler.Init();
            _playerViewRigidbody = _playerView.CharacterViewObject.GetComponent<Rigidbody2D>();
        }

        public void OnClick(Transform point, PointerEventData eventData)
        {       
            if (IsActive)
            {
                _pointerPosition = point;
                Debug.Log($"I`m {_type}! \n Wanna push me?");
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsActive && !_playerView.IsMoving)
            {
                _initPosition = _playerView.CharacterViewObject.GetComponent<Transform>().position;

                IMyPoolable slingShotPoolable = _slingShotPooler.Pull<IMyPoolable>(_type, _initPosition, _pointerPosition.rotation, _pointerPosition.parent);
                _slingShot = slingShotPoolable.gameObject.GetComponent<SlingShot>();

                _slingShot.Init(_initPosition, _type);

                if (!_isChangeDirectionSubscibed)
                {
                    _slingShot.OnDirectionChange += ChangeDirection;
                    _isChangeDirectionSubscibed = true;
                }


                if (_slingShotObject == null)
                {
                    _slingShotObject = slingShotPoolable.gameObject;
                }

                DragInputModule.dragFocusObject = _slingShotObject;
                eventData.pointerDrag = _slingShotObject;
                eventData.dragging = true;

                if (!_isLaunchWasSubscibed)
                {
                    _slingShot.OnShoot += Launch;
                    _isLaunchWasSubscibed = true;
                }
            }
        }

        public void ChangeDirection(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _playerViewRigidbody.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }


        public void Launch(Vector2 direction, float dragDistance)
        {
            _playerView.IsMoving = true;
            Vector2 forceVector = direction * dragDistance * _maxPower;

            _playerViewRigidbody.AddForce(forceVector, ForceMode2D.Impulse);
            _slingShot.OnShoot -= Launch;
            _isLaunchWasSubscibed = false;
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

            _slingShot.OnDirectionChange -= ChangeDirection;
            _isChangeDirectionSubscibed = true;
        }
    }


}
