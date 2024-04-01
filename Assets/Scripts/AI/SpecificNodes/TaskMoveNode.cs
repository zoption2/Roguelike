using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class TaskMoveNode : Node
    {
        public override NodeState Evaluate()
        {
            _characterController.Move();
            _state = NodeState.Success;
            return _state;
        }
    }
}
