using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Selector : Node
    {
        public Selector() : base()
        {

        }

        public Selector(List<Node> children) : base(children)
        {

        }

        public override NodeState Evaluate()
        {
            foreach (Node child in _children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Failure:
                        continue;
                    case NodeState.Success:
                        return _state;
                    case NodeState.Running:
                        _state = NodeState.Running;
                        return _state;
                }
            }

            _state = NodeState.Failure;
            return _state;
        }
    }
}
