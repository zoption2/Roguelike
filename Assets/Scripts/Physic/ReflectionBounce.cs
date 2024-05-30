using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionBounce : IMovable
{
    private Rigidbody _providerRb;
    private Rigidbody _handlerRb;
    private Vector3 _providerVelocity;
    private Vector3 _handlerVelocity;
    public void ApplyForce(IInteractible provider, IInteractible handler)
    {
        _providerRb = provider.GetRigidbody();
        _handlerRb = handler.GetRigidbody();
        _providerVelocity = provider.GetVelocity();
        _providerVelocity.z = 0;
        _handlerVelocity = handler.GetVelocity();
        _handlerVelocity.z = 0;
        Vector2 normal;

        normal = provider.Normal;

        Vector3 reflectedVelocity = Vector3.Reflect(_providerVelocity, normal);

        _handlerRb.velocity = _providerVelocity;
        if (_providerRb.velocity.magnitude < 1 && _handlerRb.velocity.magnitude == 0)
        {
            _providerRb.velocity =  reflectedVelocity * 2;
        }
        else
        {
            _providerRb.velocity = reflectedVelocity * 1.5f;
        }
    }
}
