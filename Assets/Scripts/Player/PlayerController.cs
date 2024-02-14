using Gameplay;
using Pool;
using Prefab;
using SlingShotLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
        public void Init(Transform point, PlayerType type);
    }
    public class PlayerController : IPlayerController
    {
        private IPlayerView _playerView;
        private PlayerModel _playerModel;

        private Rigidbody2D _playerViewRigidbody;

        private SlingShot _slingShot;

        private GameObject _slingShotObject;

        private float _maxPower = 10f;

        public delegate void OnSwitchState();
        public static event OnSwitchState OnSwitch;

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
        public  void  Init(Transform point, PlayerType type)
        {
            _playerPooler.Init();
            _slingShotPooler.Init();
            var poolable = _playerPooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
            _playerView = poolable.gameObject.GetComponent<PlayerView>();
            _playerViewRigidbody = poolable.gameObject.GetComponentInChildren<Rigidbody2D>(); 
            _playerView.Initialize(this);
            _playerModel = new PlayerModel();
        }
        public void OnClick(Transform point, SlingShotType type, PointerEventData eventData)
        {
            if (IsActive)
            {
                var slingShotPoolable = _slingShotPooler.Pull<IMyPoolable>(type, point.position, point.rotation, point.parent);
                _slingShot = slingShotPoolable.gameObject.GetComponent<SlingShot>();

                _slingShot.Init();

                if(_slingShotObject == null)
                {
                    _slingShotObject = slingShotPoolable.gameObject;
                }

                DragInputModule.dragFocusObject = _slingShotObject;
                eventData.pointerDrag = _slingShotObject;
                eventData.dragging = true;
            }
            //MAKE SUBSCRIBE CHECK!
            _slingShot.OnShoot += Launch;
        }

        public void Launch(Vector2 direction, float dragDistance)
        {
            Vector2 forceVector = direction * dragDistance * _maxPower;

            _playerViewRigidbody.AddForce(forceVector, ForceMode2D.Impulse);
            _slingShot.OnShoot -= Launch;
        }
    }
}
