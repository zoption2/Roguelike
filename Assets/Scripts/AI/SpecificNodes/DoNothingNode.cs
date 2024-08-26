namespace BehaviourTree
{
    public class DoNothingNode : Node
    {
        public override NodeState Evaluate()
        {
            _characterController.HandleStopMovement();
            _state = NodeState.Success;
            return _state;
        }
    }
}
