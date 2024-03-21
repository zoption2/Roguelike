using Interactions;
using Obstacles;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CollidingObject : MonoBehaviour
{
    [SerializeField] CharacterView _characterView;

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
        _rigidbody = _characterView.Rigidbody;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.TryGetComponent(out IObstacle obstacle))
        {
            Vector3 velocity = GetVelocity();
            obstacle.ProcessCollision(_rigidbody, velocity);
        }

        if (collision.gameObject.TryGetComponent(out IInteractible interactible))
        {
            _characterView.GetInterationHandler(interactible);
        }

    }
}
