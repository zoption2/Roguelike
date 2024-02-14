using Enemy;
using Pool;
using SlingShotLogic;
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

        public event Action<PointerEventData> OnDragChange;
    }
    public class PlayerView : MonoBehaviour, IPointerDownHandler,IPlayerView, IMyPoolable
    {
        public event Action<PointerEventData> OnDragChange;

        private GameObject _slingShotObject;

        private SlingShot _slingShot;

        private PlayerController _playerController;

        private Transform _transform;

        public void Initialize(PlayerController playerController)
        {
            _playerController = playerController;
            //_playerController.OnDragChange += DragInputChanger;
        }

        public void Lauch(float power, Vector2 direction)
        {
            Debug.Log("Nothing happened yet");
        }

        public void OnCreate()
        {
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _transform = transform;
            //OnDragChange?.Invoke(eventData);
            //_playerController.OnDragChange += DragInputChanger;
            _playerController.OnClick(_transform, Prefab.SlingShotType.Melee, eventData);
            //_slingShot.Init();
            //_playerController.OnDragChange += DragInputChanger;
        }

        public void OnPull()
        {
            
        }

        public void OnRelease()
        {
        }



       
    }
}
