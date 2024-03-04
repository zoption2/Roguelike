﻿using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Pool;
using Prefab;
using Zenject;

namespace SlingShotLogic
{
    public interface ISlingShot
    {
        public void Init(Vector2 _initPosition, SlingShotType type);
        public event Action<Vector2, float> OnShoot;
    }

    public class SlingShot : MonoBehaviour, ISlingShot, IDragHandler, IEndDragHandler, IMyPoolable
    {
        public event Action<Vector2, float> OnShoot;

        [SerializeField] GameObject _cursor;
        [SerializeField] GameObject _touchZone;

        private Vector2 _direction;
        private Vector2 _startPoint;
        private Vector2 _endPoint;
        private Vector2 _touchPositionInWorld;
        private SlingShotType _type;

        private Collider2D _touchZoneCollider;

        private float _dragDistance;

        public bool _isDragging = false; //ITS FOR COROUTINE

        [Inject]
        private ObjectPooler<SlingShotType> _slingShotPooler;
        public void Init(Vector2 _initPosition, SlingShotType type)
        {  
            _type = type;
            _startPoint = _initPosition;
            _cursor.transform.position = _startPoint;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _isDragging = true;
            _touchPositionInWorld = Camera.main.ScreenToWorldPoint(eventData.position);
            StopAllCoroutines();

            if (Vector2.Distance(_touchZoneCollider.bounds.center, _touchPositionInWorld) <= _touchZoneCollider.bounds.size.x / 2)
            {
                _cursor.transform.position = _touchPositionInWorld;
                _endPoint = _cursor.transform.position;
                _direction = _startPoint - _endPoint;
                //Debug.Log("Direction: " + _direction);
                //Debug.Log("Start: " + _startPoint);
                //Debug.Log("End: " + _endPoint);
            }
            else
            {
                Vector2 clampedPosition = _touchZoneCollider.ClosestPoint(_touchPositionInWorld);
                _cursor.transform.position = clampedPosition;
                _endPoint = clampedPosition;
                _direction = _startPoint - _endPoint;
            }

        }
        public void OnEndDrag(PointerEventData eventData)
        {
            if(_isDragging )
            {
                if (IsInDeadZone(_touchPositionInWorld))
                {
                    _slingShotPooler.Push(_type, this);
                }
                else
                {
                    _dragDistance = Vector2.Distance(_startPoint, _endPoint);
                    OnShoot?.Invoke(_direction, _dragDistance);
                    _slingShotPooler.Push(_type, this);
                }
            }
            _isDragging = false;
        }

        private bool IsInDeadZone(Vector2 position)
        {
            float innerRadius = _touchZoneCollider.bounds.size.x / 3f * 0.3f;
            return Vector2.Distance(position, _touchZoneCollider.bounds.center) < innerRadius;
        }

        public void OnCreate()
        {
            _touchZoneCollider = _touchZone.GetComponent<Collider2D>();
        }

        public void OnPull()
        {
            //Debug.Log("OnPull");
        }

        public void OnRelease()
        {
            //Debug.Log("OnRelease");
        }

    }
}