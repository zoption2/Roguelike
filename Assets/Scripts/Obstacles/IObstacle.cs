using Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obstacles
{
    public interface IObstacle
    {
        //public void ProcessCollision(Rigidbody rigidbody, Vector3 velocity);
    }

    public interface IWall : IObstacle
    {
        public void ProcessCollision(Rigidbody rigidbody, Vector3 velocity);
    }

    public interface IBuff : IObstacle
    {
        public List<IEffect> ProcessTrigger();
    }

}



