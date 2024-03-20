using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree {

    public abstract class ConditionalNode : Node
    {
        protected abstract string _key { get; }
        public override NodeState Evaluate()
        {
            bool value = (bool)_blackboard.GetData(_key);
            if (value)
            {
                _state = NodeState.Success;
                return _state;
            }
            else
            {
                _state = NodeState.Failure;
                return _state;
            }
        }
    }
    public class CanAttackNode : ConditionalNode
    {
        protected override string _key { get; } = "CanAttack";
        
    }
}
