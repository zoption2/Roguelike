using Enemy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
    public interface IPlayerView
    {
        public void Initialize(PlayerController playerController);
    }
    public class PlayerView : MonoBehaviour, IPointerDownHandler,IPlayerView
    {
        private PlayerController _playerController;

        public void Initialize(PlayerController playerController)
        {
            _playerController = playerController;
        }

        public void Lauch(float power, Vector2 direction)
        {
            Debug.Log("Nothing happened yet");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _playerController.OnClick();
        }
    }
}
