using UnityEngine;

namespace Obstacles
{
    public class DistortionWall : MonoBehaviour, IWall
    {
        public void ProcessCollision(Collision collision, Rigidbody rigidbody, Vector3 velocity)
        {
            Vector3 reflectionVector = collision.GetContact(0).normal;

            Vector3 reflectedVelocity = Vector3.Reflect(new Vector3(velocity.x, 0, velocity.z), reflectionVector);
            rigidbody.velocity = reflectedVelocity;
        }
    }
}
