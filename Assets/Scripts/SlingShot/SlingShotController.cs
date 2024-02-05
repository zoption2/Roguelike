using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SlingShot
{
    public class SlingShotController : MonoBehaviour
    {
        [SerializeField] float _power;
        [SerializeField] Rigidbody2D _rigidBody;
        [SerializeField] Vector2 _minPower;
        [SerializeField] Vector2 _maxPower;
        SlingShotLine _slingShotLine;

        Camera _camera;
        Vector2 _force;
        Vector2 _startPoint;
        Vector2 _endPoint;

        private bool _isInMotion = false;

        private void Start()
        {
            _camera = Camera.main;
            _slingShotLine = GetComponent<SlingShotLine>();
        }

        private void Update()
        {
            if (_rigidBody.velocity.magnitude < 0.1f)
            {


                _isInMotion = false;



            }
            else { _isInMotion = true; }

            if (! _isInMotion )
            {
                if (Input.GetMouseButtonDown(0))
                {

                    _startPoint = transform.position;

                }

                if (Input.GetMouseButton(0))
                {
                    Vector2 _currentPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
                    //Debug.Log("Зараз я тут: " + _currentPoint);

                    _slingShotLine.RenderLine(_startPoint, _currentPoint);
                }

                if (Input.GetMouseButtonUp(0))
                {

                    _endPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
                    //Debug.Log("Кінець: " + _endPoint);
                    Vector2 _direction = _startPoint - _endPoint;
                    _direction.Normalize();
                    _force = new Vector2(Math.Clamp(_direction.x, _minPower.x, _maxPower.x), Math.Clamp(_direction.y, _minPower.y, _maxPower.y));

                    _rigidBody.AddForce(_force * _power, ForceMode2D.Impulse);


                    _slingShotLine.EndLine();
                }
            }
            

            
            Debug.Log("Рух? " + _isInMotion);
        }
    }
}
