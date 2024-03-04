using Pool;
using Prefab;
using SlingShotLogic;
using System;
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
        Transform _poolableTransform;
        Transform _pointerPosition;

        private ICharacterView _playerView;
        private CharacterModel _playerModel;

        private Rigidbody2D _playerViewRigidbody;

        private SlingShot _slingShot;
        private SlingShotType _slingShotType;

        private GameObject _slingShotObject;

        private float _maxPower = 50f;

        private bool _isLaunchWasSubscibed;

        public event OnSwitchState OnSwitch;

        [Inject]
        private ObjectPooler<SlingShotType> _slingShotPooler;
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
            _playerView.ON_CLICK += OnClick;
            _playerView.ON_BEGINDRAG += OnBeginDrag;
            _slingShotType = (SlingShotType)_playerModel.GetModelType();
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
                IMyPoolable slingShotPoolable = _slingShotPooler.Pull<IMyPoolable>(_slingShotType, _initPosition, _pointerPosition.rotation, _pointerPosition.parent);
                _slingShot = slingShotPoolable.gameObject.GetComponent<SlingShot>();

                _slingShot.Init(_initPosition, _slingShotType);


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
            Vector2 forceVector = direction * dragDistance * _maxPower;

            _playerViewRigidbody.AddForce(forceVector, ForceMode2D.Impulse);
            _slingShot.OnShoot -= Launch;
            _isLaunchWasSubscibed = false;
            OnSwitch.Invoke();

        }

        public void Dispose()
        {
            _playerView.ON_CLICK -= OnClick;
            _playerView.ON_BEGINDRAG -= OnBeginDrag;
        }
    }
}
