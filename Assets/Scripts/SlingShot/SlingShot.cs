using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace SlingShotLogic
{
    public interface ISlingShot
    {
        public void Init();
        public event Action<Vector2, float> OnShoot;
    }

    public class SlingShot : MonoBehaviour, ISlingShot, IDragHandler, IEndDragHandler
    {
        public event Action<Vector2, float> OnShoot;

        [SerializeField] GameObject _cursor;
        [SerializeField] GameObject _touchZone;

        private Vector2 _direction;
        private Vector2 _startPoint;
        private Vector2 _endPoint;
        private Vector2 _touchPositionInWorld;

        private Collider2D _touchZoneCollider;

        private float _dragDistance;

        private bool _isDragging = false;//DELETE WHEN STATE SYSTEM WILL READY//
        public void Init()
        {
            gameObject.SetActive(true);

            _touchZoneCollider = _touchZone.GetComponent<Collider2D>();
            _startPoint = transform.position;
            _cursor.transform.position = _startPoint;

            if(!_isDragging) StartCoroutine(DeactivateAfterDelay(0.3f)); ///!!!!///
        }

        /////////////////////////////DELETE WHEN STATE SYSTEM WILL READY///////////////////////////////////
        private System.Collections.IEnumerator DeactivateAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }
        /////////////////////////////DELETE WHEN STATE SYSTEM WILL READY///////////////////////////////////

        public void OnDrag(PointerEventData eventData)
        {
            _isDragging = true;
            _touchPositionInWorld = Camera.main.ScreenToWorldPoint(eventData.position);
            StopAllCoroutines();

            if (_touchZoneCollider.bounds.Contains(_touchPositionInWorld))
            {
                _cursor.transform.position = _touchPositionInWorld;
                _endPoint = _cursor.transform.position;
                _direction = _startPoint - _endPoint;
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
            _isDragging = false;

            if (IsInDeadZone(_touchPositionInWorld))
            {
                gameObject.SetActive(false);
                
            } else
            {
                _dragDistance = Vector2.Distance(_startPoint, _endPoint);
                OnShoot?.Invoke(_direction, _dragDistance);
                gameObject.SetActive(false);
            }
            
        }

        private bool IsInDeadZone(Vector2 position)
        {
            float innerRadius = _touchZoneCollider.bounds.size.x / 2f * 0.2f;
            return Vector2.Distance(position, _touchZoneCollider.bounds.center) < innerRadius;
        }

    }
}