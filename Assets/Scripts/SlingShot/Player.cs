using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace SlingShot
{
    public interface IPlayer
    {
        public void Launch(Vector2 _direction, float _force);
    }

    public class Player : MonoBehaviour, IPointerDownHandler, IPlayer, IDragHandler
    {
        private Rigidbody2D _rigidbody;
        private Vector2 _position;
        private float _maxPower = 10f;
        private bool isLaunchSubscribed = false;

        public Action onTouchStart;
        public GameObject slingShotControllerPrefab;

        GameObject slingShotControllerObject;
        SlingShotController slingShotController;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _position = transform.position;

            if (slingShotController == null)
            {
                slingShotControllerObject = Instantiate(slingShotControllerPrefab, _position, Quaternion.identity);
                slingShotController = slingShotControllerObject.GetComponent<SlingShotController>();

                slingShotController.Init();

                DragInputModule.dragFocusObject = slingShotControllerObject;

                if (!isLaunchSubscribed)
                {
                    slingShotController.OnShoot += Launch;
                    isLaunchSubscribed = true;
                }

            } else
            {
                slingShotController.transform.position = _position;

                if (_rigidbody.velocity.magnitude != 0f) return;
                
                slingShotController.Init();
                
                DragInputModule.dragFocusObject = slingShotControllerObject;

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

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("Hello there!");
        }
    }

}
