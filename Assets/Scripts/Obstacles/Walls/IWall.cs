using UnityEngine;

namespace Obstacles
{
    public interface IWall
    {
        public void ProcessCollision(Rigidbody rigidbody, Vector3 velocity);
    }
} 

