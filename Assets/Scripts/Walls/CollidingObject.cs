using Obstacles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidingObject : MonoBehaviour
{
    private Rigidbody _rigidbody;
    Queue<Vector3> _lastVelocities = new(2);
    void FixedUpdate()
    {
        _lastVelocities.Enqueue(_rigidbody.velocity);

        if(_lastVelocities.Count > 2 )
        {
            _lastVelocities.Dequeue();
        }
    }

    public Vector3 GetVelocity()
    {
        return _lastVelocities.Dequeue();
    }


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.TryGetComponent(out IObstacle obstacle))
        {
            Vector3 velocity = GetVelocity();
            Debug.LogWarning(velocity);
            obstacle.ProcessCollision(_rigidbody, velocity);
            Debug.Log("Collision processed");
        }

    }
}
