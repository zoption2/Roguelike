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

        private PlayerController _playerController;

        private Transform _transform;

        public void Initialize(PlayerController playerController)
        {
            _playerController = playerController;
        }

        public void OnCreate()
        {
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _transform = transform;
            _playerController.OnClick(_transform, Prefab.SlingShotType.Melee, eventData);
        }

        public void OnPull()
        {
            
        }

        public void OnRelease()
        {
        }



       
    }
}
