using UnityEngine;

public class Bounce : IMovable
{
    private Rigidbody _providerRb;
    private Rigidbody _handlerRb;

    public void ApplyForce(Rigidbody rb1, Rigidbody rb2)
    {
        _providerRb = rb1;
        _handlerRb = rb2;

        _handlerRb.velocity = _providerRb.velocity;

        _providerRb.velocity = -_providerRb.velocity;
    }
}
