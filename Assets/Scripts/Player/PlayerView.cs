using Assets.Scripts.PrefabProvider;
using Enemy;
using SlingShotLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;
using Zenject;

namespace Player
{
    public interface IPlayerView
    {
        public void Initialize(PlayerController playerController);
    }
    public class PlayerView : MonoBehaviour, IPlayerView, IPointerDownHandler
    {
        private PlayerController _playerController;
        private Transform _transform;

        private Rigidbody2D _rigidbody;
        private Vector2 _position;
        private float _maxPower = 10f;
        private bool isLaunchSubscribed = false;
        private PrefabProviderByType _prefabProvider;
        private GameObject _slingShotControllerPrefab;

        public Action onTouchStart;

        GameObject slingShotControllerObject;
        SlingShot slingShotController;

        public void Initialize(PlayerController playerController)
        {
            _playerController = playerController;
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _transform = transform;
            DragInputChanger(eventData);
            _playerController.OnClick(_transform, Prefab.SlingShotType.Melee);
        }
        public void Launch(Vector2 direction, float dragDistance)
        {
            Vector2 forceVector = direction * dragDistance * _maxPower;
            _rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
            slingShotController.OnShoot -= Launch;
            isLaunchSubscribed = false;
        }

        private void DragInputChanger(PointerEventData eventData)
        {
            DragInputModule.dragFocusObject = slingShotControllerObject;
            eventData.pointerDrag = slingShotControllerObject;
            eventData.dragging = true;
        }
    }
}