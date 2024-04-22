using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class DoNothingNode : Node
    {
        public override NodeState Evaluate()
        {
            Debug.Log("!!!Turn was skipped!!!");
            _characterController.SkipTurn();
            _state = NodeState.Success;
            return _state;
        }
    }
}
