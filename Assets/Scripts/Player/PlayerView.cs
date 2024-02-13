﻿using Assets.Scripts.PrefabProvider;
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
    public interface IPlayer
    {
        public void Launch(Vector2 _direction, float _force);
    }
    public class PlayerView : MonoBehaviour, IPlayer, IPointerDownHandler
    {
        private PlayerController _playerController;

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

            _prefabProvider = FindObjectOfType<PrefabProviderByType>();

            _slingShotControllerPrefab = _prefabProvider.GetPrefab<SlingShot>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _position = transform.position;

            if (slingShotController == null)
            {
                slingShotControllerObject = Instantiate(_slingShotControllerPrefab, _position, Quaternion.identity);
                slingShotController = slingShotControllerObject.GetComponent<SlingShot>();

                slingShotController.Init();

                DragInputChanger(eventData);

                if (!isLaunchSubscribed)
                {
                    slingShotController.OnShoot += Launch;
                    isLaunchSubscribed = true;
                }

            }
            else
            {
                slingShotController.transform.position = _position;

                if (_rigidbody.velocity.magnitude != 0f) return;// !!!!!!!DELETE WHAN STATE WILL WORK!!!!!!!!!

                slingShotController.Init();

                DragInputChanger(eventData);

                if (!isLaunchSubscribed)
                {
                    slingShotController.OnShoot += Launch;
                    isLaunchSubscribed = true;
                }

            }
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
