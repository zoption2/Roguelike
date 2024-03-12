using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obstacles;

namespace Obstacles
{
    public class ReflectionWall : MonoBehaviour, IObstacle
    {
        public void ProcessCollision(Rigidbody rigidbody)
        {
            var velocity = rigidbody.velocity;
            rigidbody.velocity = Vector3.zero;
            var modifiedVelocity = velocity * -2;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(modifiedVelocity, ForceMode.VelocityChange);
        }
    }
}
