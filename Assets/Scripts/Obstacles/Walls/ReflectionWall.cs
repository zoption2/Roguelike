using UnityEngine;

namespace Obstacles
{
    public class ReflectionWall : MonoBehaviour, IWall
    {
        public void ProcessCollision(Collision collision, Rigidbody rigidbody, Vector3 velocity, ParticlePooler particlePooler = null)
        {
            Vector3 modifiedVelocity = velocity * -1.5f;
            rigidbody.velocity = modifiedVelocity;
        }
    }
}
