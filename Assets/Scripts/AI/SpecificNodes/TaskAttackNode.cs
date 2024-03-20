using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class TaskAttackNode : Node
    {
        public override NodeState Evaluate()
        {
            // do some stuff
            // check if everything went okay
            _state = NodeState.Success;
            Debug.Log("Attack was successful") ;
            return _state;
        }
    }
}
