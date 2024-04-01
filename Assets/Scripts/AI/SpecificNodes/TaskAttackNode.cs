using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class TaskAttackNode : Node
    {
        public override NodeState Evaluate()
        {
            _characterController.Attack();
            _state = NodeState.Success;
            return _state;
        }
    }
}
