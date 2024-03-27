using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Sequence : Node
    {
        public Sequence() : base()
        {
            
        }

        public Sequence(List<Node> children) : base(children)
        {
            
        }

        public override NodeState Evaluate()
        {
            bool someoneRunning = false;

            foreach (Node child in _children)
            {
                switch(child.Evaluate())
                {
                    case NodeState.Failure:
                        _state = NodeState.Failure;
                        return _state;
                    case NodeState.Success:
                        continue;
                    case NodeState.Running:
                        someoneRunning = true;
                        continue;
                }
            }

            if (someoneRunning)
                _state = NodeState.Running;
            else 
                _state = NodeState.Success;
            return _state;
        }
    }
}
