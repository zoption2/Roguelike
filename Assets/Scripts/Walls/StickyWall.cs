using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obstacles
{
    public class StickyWall : MonoBehaviour, IObstacle
    {
        public void ProcessCollision(Rigidbody rigidbody)
        {
            rigidbody.velocity = Vector3.zero;
        }
    }
}

