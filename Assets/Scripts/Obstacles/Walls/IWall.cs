using UnityEngine;

namespace Obstacles
{
    public interface IWall
    {
        public void ProcessCollision(Collision collision, Rigidbody rigidbody, Vector3 velocity, ParticlePooler particlePooler = null);
    }
} 

