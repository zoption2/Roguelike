using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Player
{
    public class PlayerView : MonoBehaviour, IPointerDownHandler
    {
        private PlayerController _playerController;

        public void Initialize(PlayerController playerController)
        {
            _playerController = playerController;
        }

        public void Lauch(/*Power, direction and etc...*/)
        {
            Debug.Log("Nothing happened yet");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _playerController.OnClick();
        }
    }

}
