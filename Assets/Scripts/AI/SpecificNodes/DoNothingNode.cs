using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class DoNothingNode : Node
    {
        public override NodeState Evaluate()
        {
            _characterController.SkipTurn();
            Debug.Log("!!!Turn was skipped!!!");
            _state = NodeState.Success;
            return _state;
        }
    }
}
