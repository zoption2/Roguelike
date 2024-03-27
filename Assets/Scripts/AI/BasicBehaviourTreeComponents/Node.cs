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
    public abstract class Node
    {
        protected Node _parent;
        protected List<Node> _children;
        protected NodeState _state;
        protected Blackboard _blackboard;
        protected ICharacterController _characterController;

        public Node()
        {
            _parent = null;
            _children = new List<Node>();
        }

        public Node(List<Node> children)
        {
            _children = new List<Node>();
            foreach (Node child in children)
            {
                AttachChildNode(child);
            }
        }

        public void SetCharacter(ICharacterController characterController)
        {
            _characterController = characterController;
        }
        public void SetBlackboard(Blackboard blackboard)
        {
            _blackboard = blackboard;
        }
        public abstract NodeState Evaluate();

        public void AttachChildNode(Node childNode)
        {
            if (!_children.Contains(childNode))
            {
                childNode._parent = this;
                _children.Add(childNode);
            }
        }

        public bool HasChildren()
        {
            return _children.Count > 0;
        }

        public List<Node> GetChildren()
        {
            return _children;
        }
    }
}
