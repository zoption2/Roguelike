using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree 
{

    public enum NodeState
    {
        Failure=0,
        Success=1,
        Running=2
    }
    public class Node
    {
        protected Node _parent;
        protected List<Node> _children;
        protected NodeState _state;

        public Node()
        {
            _parent = null;
        }

        public Node(List<Node> children)
        {
            foreach (Node child in children)
            {
                AttachChildNode(child);
            }
        }

        public virtual NodeState Evaluate() => NodeState.Failure;

        public void AttachChildNode(Node childNode)
        {
            childNode._parent = this;
            _children.Add(childNode);
        }
    }
}
