using UnityEngine;

namespace Obstacles
{
    public class DistortionWall : MonoBehaviour, IWall
    {
        public void ProcessCollision(Collision collision, Rigidbody rigidbody, Vector3 velocity)
        {
            //Debug.LogWarningFormat("Hit with wall! {0},{1}", System.DateTime.Now, System.DateTime.Now.Millisecond);
            //Debug.LogWarningFormat("Hit with wall time: {0} velocity: {1}", System.DateTime.Now.Millisecond, velocity);
            Vector3 reflectionVector = collision.GetContact(0).normal;
            Debug.LogWarning(reflectionVector);

            Vector3 reflectedVelocity = Vector3.Reflect(new Vector3(velocity.x, 0, velocity.z), reflectionVector);
            //Debug.LogWarningFormat("reflectedVelocity: {0}", reflectedVelocity);
            rigidbody.velocity = reflectedVelocity;
        }
    }
}
