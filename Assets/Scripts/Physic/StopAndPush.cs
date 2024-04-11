using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StopAndPush : IMovable
{
    private Rigidbody _providerRb;
    private Rigidbody _handlerRb;

    public void ApplyForce(Rigidbody providerRb, Rigidbody handlerRb)
    {
        _providerRb = providerRb;
        _handlerRb = handlerRb;

        _handlerRb.velocity = _providerRb.velocity * 2;
        _handlerRb.angularVelocity = _providerRb.angularVelocity;

        _providerRb.velocity = Vector3.zero;
        _providerRb.angularVelocity = Vector3.zero;
    }
}
