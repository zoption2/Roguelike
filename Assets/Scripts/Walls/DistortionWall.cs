using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obstacles
{

    public class DistortionWall : MonoBehaviour, IObstacle
{
        public void ProcessCollision(Rigidbody rigidbody, Vector3 velocity)
        {
            var modifiedVelocity = new Vector3(-velocity.x, velocity.y, -velocity.z);
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(modifiedVelocity, ForceMode.VelocityChange);
        }
    }
}

