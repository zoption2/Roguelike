using Gameplay;
using Pool;
using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        public delegate void OnSwitchState();
        public static event OnSwitchState OnSwitch;

        [Inject]
        private ObjectPooler<IPlayerView,PlayerType> _playerPooler;
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
            _playerView = _playerPooler.GetElementAndSpawnIfWasntSpawned<IPlayerView>(type ,point.position,point.rotation,point.parent);
            _playerView.Initialize(this);
            _playerModel = new PlayerModel();
        }
        public void OnClick()
        {
            if (IsActive)
            {
                Debug.Log("Player was clicked");
                OnSwitch?.Invoke();
                //Switch state
            }
                
        }
    }
}
