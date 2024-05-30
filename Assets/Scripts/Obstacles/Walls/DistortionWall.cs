using UnityEngine;

namespace Obstacles
{
    public class DistortionWall : MonoBehaviour, IWall
    {
        public void ProcessCollision(Collision collision, Rigidbody rigidbody, Vector3 velocity)
        {

            Vector3 reflectionVector = collision.GetContact(0).normal;

            Vector3 reflectedVelocity = Vector3.Reflect(velocity, reflectionVector);

            rigidbody.velocity = reflectedVelocity;
        }
    }
}
