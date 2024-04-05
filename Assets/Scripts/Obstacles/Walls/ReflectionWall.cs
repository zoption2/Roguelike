using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obstacles;

namespace Obstacles
{
    public class ReflectionWall : MonoBehaviour, IWall
    {
        public void ProcessCollision(Collision collision, Rigidbody rigidbody, Vector3 velocity)
        {
            rigidbody.velocity = Vector3.zero;
            var modifiedVelocity = velocity * -1.5f;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(modifiedVelocity, ForceMode.VelocityChange);
        }
    }
}
