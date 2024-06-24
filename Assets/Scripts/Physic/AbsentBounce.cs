using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsentBounce : IMovable
{
    private Rigidbody _providerRb;
    private Rigidbody _handlerRb;

    public void ApplyForce(IInteractible provider, IInteractible handler)
    {
        _providerRb = provider.GetRigidbody();
        _handlerRb = handler.GetRigidbody();

        _handlerRb.velocity = Vector3.zero;
        _handlerRb.angularVelocity = Vector3.zero;
        _providerRb.velocity = Vector3.zero;
        _providerRb.angularVelocity = Vector3.zero;
    }
}
