using UnityEngine;


public class StopAndPush : IMovable
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
        _handlerVelocity = handler.GetLastVelocity();

        _handlerRb.velocity = _providerVelocity;

        _providerRb.velocity = Vector3.zero;
        _providerRb.angularVelocity = Vector3.zero;
    }
}
