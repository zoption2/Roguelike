using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Pool;
using Zenject;

namespace SlingShotLogic
{
    public interface ISlingShot : IMyPoolable
    {
        public void Init(Vector3 _initPosition, CharacterType type, float currentLaunchDistance);
        public void UnsubscribeEvents();
        public event Action<Vector3> OnShoot;
        public event Action OnAbilityUse;
        public event Action<Vector3> OnDirectionChange;
    }

    public class SlingShot : MonoBehaviour, ISlingShot, IDragHandler, IEndDragHandler
    {
        public event Action<Vector3> OnShoot;
        public event Action OnAbilityUse;
        public event Action<Vector3> OnDirectionChange;
        public bool IsDragging = false;

        [SerializeField] Image _cursor;
        [SerializeField] Image _touchZone;
        [SerializeField] RectTransform _pointer;
        [SerializeField] RectTransform _pointerOrigin;

        private Vector3 _direction;
        private Vector3 _startPoint;
        private Vector3 _endPoint;
        private Vector3 _touchPositionInWorld;
        private CharacterType _type;

        [Inject]
        private IPoolManager _poolManager;
        private SlingshotPooler _slingShotPooler;
        private float _launchDistance;
        private const float SLINGSHOT_RADIUS = 2.1f;
        private const float POINTER_RADIUS = 0.5f;

        public void Init(Vector3 _initPosition, CharacterType type, float currentLaunchDistance)
        {
            _slingShotPooler = _poolManager.UseSlingshotPooler();
            _type = type;
            _startPoint = _initPosition;
            _launchDistance = currentLaunchDistance;
        }

        public void OnDrag(PointerEventData eventData)
        {
            IsDragging = true;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_touchZone.rectTransform, eventData.position, Camera.main, out _touchPositionInWorld);

            _touchPositionInWorld.y = _startPoint.y;

            Vector3 clampedPosition = ClampToCircle(_touchPositionInWorld, _touchZone.rectTransform, _cursor.rectTransform);
            _cursor.rectTransform.position = new Vector3(clampedPosition.x, _cursor.rectTransform.position.y, clampedPosition.z);
            _endPoint = _cursor.rectTransform.position;
            _direction = _startPoint - _endPoint;
            ChangePointerDirection(_direction);
            ChangePointerLength(_direction.magnitude, _direction);
            OnDirectionChange?.Invoke(_direction);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (IsDragging)
            {
                if (IsInDeadZone(_cursor.rectTransform.position, _touchZone.rectTransform))
                {
                    UnsubscribeEvents(); // Відписуємо події перед поверненням в пул
                    _slingShotPooler.Push(_type, this);
                }
                else
                {
                    OnShoot?.Invoke(_direction);
                    OnAbilityUse?.Invoke();
                    UnsubscribeEvents(); // Відписуємо події перед поверненням в пул
                    _slingShotPooler.Push(_type, this);
                }
            }
            IsDragging = false;
        }

        public void UnsubscribeEvents()
        {
            OnShoot = null;
            OnAbilityUse = null;
            OnDirectionChange = null;
        }

        private bool IsInDeadZone(Vector3 position, RectTransform zone)
        {
            float innerRadius = zone.rect.width / 3f * 0.3f;
            return Vector3.Distance(new Vector3(position.x, 0, position.z), new Vector3(zone.position.x, 0, zone.position.z)) < innerRadius;
        }

        private Vector3 ClampToCircle(Vector3 position, RectTransform zone, RectTransform cursor)
        {
            Vector3 zoneCenter = new Vector3(zone.position.x, 0, zone.position.z);
            Vector3 direction = new Vector3(position.x, 0, position.z) - zoneCenter;
            float zoneRadius = zone.rect.width / 2;
            float cursorRadius = cursor.rect.width / 2;
            float maxDistance = zoneRadius - cursorRadius;

            if (direction.magnitude > maxDistance)
            {
                direction = direction.normalized * maxDistance;
            }
            return zoneCenter + direction;
        }

        public void ChangePointerDirection(Vector3 direction)
        {
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            _pointerOrigin.localEulerAngles = Vector3.forward * -angle;
        }

        public void ChangePointerLength(float directionMagnitude, Vector3 direction)
        {
            float length = _launchDistance * (directionMagnitude / SLINGSHOT_RADIUS);
            RaycastHit hit;

            if (Physics.SphereCast(transform.position, POINTER_RADIUS, direction, out hit))
            {
                length = Mathf.Min(length, hit.distance);
            }

            Vector2 size = new Vector2(1, length);
            Vector2 position = new Vector2(0, length / 2);
            _pointer.sizeDelta = size;
            _pointer.anchoredPosition = position;
        }

        public void OnCreate()
        {
        }

        public void OnPull()
        {
        }

        public void OnRelease()
        {
        }
    }
}
