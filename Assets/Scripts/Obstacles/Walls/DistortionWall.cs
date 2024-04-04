using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obstacles
{
    public class DistortionWall : MonoBehaviour, IWall
    {
        public void ProcessCollision(Rigidbody rigidbody, Vector3 velocity)
        {
            Vector3 surfaceNormal = transform.forward;
            Vector3 oppositeVelocity = -velocity;
            Vector3 distortedVelocity = Vector3.Reflect(oppositeVelocity, surfaceNormal);
            rigidbody.velocity = distortedVelocity;
        }
    }

}

