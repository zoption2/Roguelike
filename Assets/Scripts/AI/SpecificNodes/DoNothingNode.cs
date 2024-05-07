using UnityEngine;

namespace BehaviourTree
{
    public class DoNothingNode : Node
    {
        public override NodeState Evaluate()
        {
            Debug.Log("!!!Turn was skipped!!!");
            _characterController.HandleStopMovement();
            _state = NodeState.Success;
            return _state;
        }
    }
}
