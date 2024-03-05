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
    }

    public delegate void OnEndTurn();
    public class PlayerController : IPlayerController, IDisposable
    {
        private Transform _poolableTransform;
        private Transform _pointerPosition;

        private ICharacterView _playerView;
        private CharacterModel _playerModel;
        private PlayerType _type;
        private SlingshotPooler _slingShotPooler;

        private Rigidbody2D _playerViewRigidbody;

        private SlingShot _slingShot;

        private GameObject _slingShotObject;

        private float _maxPower = 50f;

        private bool _isLaunchWasSubscibed;

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
        Transform poolableTransform,
        Rigidbody2D playerViewRigidbody,
        CharacterModel playerModel,
        CharacterView playerView)
        {
            _poolableTransform = poolableTransform;
            _playerViewRigidbody = playerViewRigidbody;
            _playerModel = playerModel;
            _playerView = playerView;
            _playerView.Init(_playerModel, _playerViewRigidbody);
            _playerModel.Velocity.ToDisposableList(_disposables).Subscribe(EndTurn);
            ////////////////////////////////////////////////
            _type = _playerModel.GetModelType<PlayerType>();
            ////////////////////////////////////////////////
            _playerView.ON_CLICK += OnClick;
            _playerView.ON_BEGINDRAG += OnBeginDrag;
            _slingShotPooler.Init();
        }

        public void OnClick(Transform point, PointerEventData eventData)
        {       
            if (IsActive)
            {
                _pointerPosition = point;
                Debug.Log($"I`m {_playerModel.GetModelType<PlayerType>()}! \n Wanna push me?");
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsActive && !_playerView.IsMoving)
            {
                Vector2 _initPosition = _poolableTransform.position;
                IMyPoolable slingShotPoolable = _slingShotPooler.Pull<IMyPoolable>(_type, _initPosition, _pointerPosition.rotation, _pointerPosition.parent);
                _slingShot = slingShotPoolable.gameObject.GetComponent<SlingShot>();

                _slingShot.Init(_initPosition, _type);


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
        }
    }


}
