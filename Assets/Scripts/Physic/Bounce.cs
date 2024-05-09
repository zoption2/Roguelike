using UnityEngine;

public class Bounce : IMovable
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

        if (_providerRb.velocity.magnitude < 1 && _handlerRb.velocity.magnitude == 0)
        {
            _providerRb.velocity *= -2f;
        }
        else
        {
            _handlerRb.velocity = _providerVelocity;
            _providerRb.velocity = -_providerVelocity;
        }
    }
}
