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
using UnityEngine.UIElements;
using UnityEngine.WSA;
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
        private ISlingShot _slingShot;
        private PlayerModel _playerModel;

        public delegate void OnSwitchState();
        public static event OnSwitchState OnSwitch;

        GameObject slingShotControllerObject;

        [Inject]
        private ObjectPooler<IPlayerView,PlayerType> _playerPooler;

        [Inject]
        private ObjectPooler<ISlingShot, PlayerType> _slingShotPooler;
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
            _playerView = _playerPooler.GetElementAndSpawnIfWasntSpawned<IPlayerView>(type ,point.position,point.rotation);
            _playerView.Initialize(this);
            _playerModel = new PlayerModel();
        }
        public void OnClick(Transform transform, PlayerType type)//slingshot type
        {
            if (IsActive)
            {    
                Debug.Log("Player was clicked");
                //OnSwitch?.Invoke();
                _slingShot = _slingShotPooler.GetElementAndSpawnIfWasntSpawned<ISlingShot>(type, transform.position, Quaternion.identity);
                _slingShot.Init();
                //OnSwitch?.Invoke();
                //Switch state
            }
        }

        public void ActivateSlingShot()
        {
            //Maybe code will be here
        }

        private void DragInputChanger(PointerEventData eventData)
        {
            DragInputModule.dragFocusObject = slingShotControllerObject;
            eventData.pointerDrag = slingShotControllerObject;
            eventData.dragging = true;
        }
    }
}
