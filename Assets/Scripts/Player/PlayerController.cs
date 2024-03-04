using Pool;
using Prefab;
using SlingShotLogic;
using System;
using System.Collections;
using System.Drawing;
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

    public delegate void OnSwitchState();
    public class PlayerController : IPlayerController, IDisposable
    {
        private Transform _poolableTransform;
        private Transform _pointerPosition;

        private ICharacterView _playerView;
        private CharacterModel _playerModel;
        private PlayerType _type;

        private Rigidbody2D _playerViewRigidbody;

        private SlingShot _slingShot;

        private GameObject _slingShotObject;

        private float _maxPower = 50f;

        private bool _isLaunchWasSubscibed;

        private bool _isMooving;

        public event OnSwitchState OnSwitch;

        [Inject]
        private SlingshotPooler _slingShotPooler;
        public bool IsActive { get; set; }

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
            _type = (PlayerType)_playerModel.GetModelType();
            _playerView.ON_CLICK += OnClick;
            _playerView.ON_BEGINDRAG += OnBeginDrag;
            _slingShotPooler.Init();
        }

        public void OnClick(Transform point, PointerEventData eventData)
        {       
            if (IsActive)
            {
                _pointerPosition = point;
                Debug.Log($"I`m {_playerModel.GetModelType()}! \n Wanna push me?");
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsActive)
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
            _isMooving = false;
            Vector2 forceVector = direction * dragDistance * _maxPower;

            _playerViewRigidbody.AddForce(forceVector, ForceMode2D.Impulse);
            _slingShot.OnShoot -= Launch;
            _isLaunchWasSubscibed = false;

            _playerView.StartCheckSwitchConditionCoroutine(_playerView.CheckSwitchCondition(OnSwitch, _playerViewRigidbody));

        }

        public void Dispose()
        {
            _playerView.ON_CLICK -= OnClick;
            _playerView.ON_BEGINDRAG -= OnBeginDrag;
        }
    }


}
