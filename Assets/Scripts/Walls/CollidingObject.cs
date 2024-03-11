using Obstacles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidingObject : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("!!!!!");
        if (other.gameObject.TryGetComponent(out IObstacle obstacle))
        {
            Debug.Log("!!");
            obstacle.ProcessCollision(_rigidbody);
            Debug.Log("Collision processed");
        }


    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("!!!!!");
    //    if (collision.gameObject.TryGetComponent(out IObstacle obstacle))
    //    {
    //        Debug.Log("!!");
    //        obstacle.ProcessCollision(_rigidbody);
    //        Debug.Log("Collision processed");
    //    }
    //}
}
