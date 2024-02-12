using Gameplay;
using Pool;
using Prefab;
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

    }
    public class PlayerController : IPlayerController
    {
        private PlayerView _playerView;
        private PlayerModel _playerModel;
        public delegate void OnSwitchState();
        public static event OnSwitchState OnSwitch;

        [Inject]
        private IPooler _pooler;
        public bool IsActive { get; set; }

        public virtual void Init()
        {
            _playerView = GameObject.Find("Player").GetComponent<PlayerView>();
            _playerView.Initialize(this);
            _playerModel = new PlayerModel();
        }
        public  void  Init(PlayerView playerView, PlayerModel playerModel)
        {
            _playerView = playerView;
            _playerView.Initialize(this);
            _playerModel = playerModel;
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
