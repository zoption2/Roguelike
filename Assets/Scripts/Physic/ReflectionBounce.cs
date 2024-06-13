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
        _providerVelocity = provider.GetLastVelocity();
        _providerVelocity.y = 0;
        _handlerVelocity = handler.GetLastVelocity();
        _handlerVelocity.y = 0;
        Vector3 normal;

        normal = provider.Normal;

        Vector3 reflectedVelocity = Vector3.Reflect(_providerVelocity, normal);
        

        if (_providerRb.velocity.magnitude < 1 && _handlerRb.velocity.magnitude == 0)
        {
            _providerRb.velocity =  reflectedVelocity * 2;
        }
        else
        {
            _providerRb.velocity = reflectedVelocity;
            _handlerRb.velocity = _providerVelocity;
        }
    }
}
