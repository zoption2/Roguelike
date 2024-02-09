using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Zenject;

namespace SlingShot
{
    public interface ITouchableObject {
    }

    public class TouchableObject : MonoBehaviour, IPointerDownHandler, ITouchableObject
    {
        public Action onTouchStart;

        //public delegate void InitSlingShotControllerDelegate();
        //public static InitSlingShotControllerDelegate onInitSlingShotController;

        public GameObject slingShotControllerPrefab;

        private ISlingShot _slingShotController;

        private Rigidbody2D _rigidbody;

        private Vector2 _position;

        Vector2 _direction;
        float _force;

        
        [Inject]
        public void Construct(ISlingShot slingShotController)
        {
            _slingShotController = slingShotController;
        }
        

        /*
        private void OnEnable()
        {
            SlingShotController.onGetImpulse += ApplyImpulse;
        }

        private void OnDisable()
        {
            SlingShotController.onGetImpulse -= ApplyImpulse;
        }
        */

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _position = transform.position;
            //Instantiate(slingShotControllerPrefab, _position, Quaternion.identity);
            
            
            GameObject slingShotControllerObject = Instantiate(slingShotControllerPrefab, _position, Quaternion.identity);
            SlingShotController slingShotController = slingShotControllerObject.GetComponent<SlingShotController>();

            //slingShotController.Init();

            onTouchStart?.Invoke();
        }

        /*
        public void OnPointerUp(PointerEventData eventData)
        {
            _slingShotController.OnShoot += HandleShootEvent;
            _slingShotController.Relise();

            ApplyImpulse(_direction, _force);
            _slingShotController.OnShoot -= HandleShootEvent;
        }
        */
        public void ApplyImpulse(Vector2 _direction, float _force)
        {
            Vector2 forceVector = _direction * _force;
            _rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
        }

        private void HandleShootEvent(Vector2 direction, float force)
        {
            _direction = direction;
            _force = force;
        }
    }
}
