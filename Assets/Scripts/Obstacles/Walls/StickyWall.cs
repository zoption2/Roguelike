using UnityEngine;

namespace Obstacles
{
    public class StickyWall : MonoBehaviour, IWall
    {
        public void ProcessCollision(Collision collision, Rigidbody rigidbody, Vector3 velocity, ParticlePooler particlePooler = null)
        {
            rigidbody.velocity = Vector3.zero;
        }
    }
}

