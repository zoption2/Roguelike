using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class TaskMoveNode : Node
    {
        public override NodeState Evaluate()
        {
            // do some stuff
            // check if everything went okay
            _characterController.Move();
            _state = NodeState.Success;
            Debug.Log("Movement was successful");
            return _state;
        }
    }
}
