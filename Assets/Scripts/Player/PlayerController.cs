using Gameplay;
using Player;
using Pool;
using Prefab;
using SlingShotLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Player
{
    public interface ICharacterController
    {
        public void Init();
        public bool IsActive { get; set; }
    }

    public interface IPlayerController : ICharacterController
    {
        public void Init(Transform point, PlayerType type, PlayerModel model);
    }
    public class PlayerController : IPlayerController
    {
        Transform _poolableTransform;

        private IPlayerView _playerView;
        private PlayerModel _playerModel;

        private Rigidbody2D _playerViewRigidbody;

        private SlingShot _slingShot;

        private GameObject _slingShotObject;

        private float _maxPower = 50f;

        private bool _isLaunchWasSubscibed;

        public delegate void OnSwitchState();
        public static OnSwitchState OnSwitch;

        [Inject]
        private ObjectPooler<PlayerType> _playerPooler;

        [Inject]
        private ObjectPooler<SlingShotType> _slingShotPooler;
        public bool IsActive { get; set; }

        public virtual void Init()
        {
            _playerView = GameObject.Find("Player").GetComponent<PlayerView>();
            _playerView.Initialize(this);
            _playerModel = new PlayerModel();
        }
        public  void  Init(Transform point, PlayerType type, PlayerModel model)
        {
            _playerPooler.Init();
            _slingShotPooler.Init();
            var _poolable = _playerPooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
            _playerView = _poolable.gameObject.GetComponent<PlayerView>();

            var _poolableObj = _poolable.gameObject.transform.GetChild(0).gameObject;
            _poolableTransform = _poolableObj.GetComponent<Transform>(); 

            _playerViewRigidbody = _poolable.gameObject.GetComponentInChildren<Rigidbody2D>(); 
            _playerView.Initialize(this);
            _playerModel = model;
        }

        public void OnClick(Transform point, SlingShotType type, PointerEventData eventData)
        {
            if (IsActive)
            {
                Vector2 _initPosition = _poolableTransform.position;
                var slingShotPoolable = _slingShotPooler.Pull<IMyPoolable>(type, _initPosition, point.rotation, point.parent);
                _slingShot = slingShotPoolable.gameObject.GetComponent<SlingShot>();

                _slingShot.Init(_initPosition);


                if(_slingShotObject == null)
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
            
        }
    }
}
