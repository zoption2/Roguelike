using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obstacles
{
    public interface IObstacle
    {
        public void ProcessCollision(Rigidbody rigidbody);
    }
}



