using UnityEngine;

namespace Obstacles
{
    public class DistortionWall : MonoBehaviour, IWall
    {
        public void ProcessCollision(Collision collision, Rigidbody rigidbody, Vector3 velocity)
        {
            Vector3 reflectionVector = collision.contacts[0].normal;
            reflectionVector.y = 0;

            Vector3 reflectedVelocity = Vector3.Reflect(new Vector3(velocity.x, 0, velocity.z), reflectionVector);

            rigidbody.velocity = new Vector3(reflectedVelocity.x, velocity.y, reflectedVelocity.z);
        }
    }
}
