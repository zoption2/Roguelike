using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class PlayerController : IPlayerController
    {
        private PlayerView _playerView;
        private PlayerModel _playerModel;

        //TODO: add methods.
        public void Init()
        {
            _playerView = GameObject.Find("PlayerView").GetComponent<PlayerView>();
            _playerView.Initialize(this);
            _playerModel = new PlayerModel();
            Debug.Log("Controller was initialized successfully");
        }
        public void OnClick()
        {
            Debug.Log("Player was clicked");
        }
    }
    public interface IPlayerController
    {
        public void Init();
    }

}
