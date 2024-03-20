using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class BehaviourTree
    {
        protected Node _root;
        protected Blackboard _blackboard;

        public void InitTree()
        {
            _blackboard = new Blackboard();
            _root = SetupRootNode();
            SetupBlackboard(_root);
        }
        protected abstract Node SetupRootNode();
        protected abstract void UpdateBlackboard();

        protected void SetupBlackboard(Node node)
        {
            if(node.GetType() != typeof(Selector) && node.GetType() != typeof(Sequence))
            {
                node.SetBlackboard(_blackboard);
            }

            if (node.HasChildren())
            {
                foreach(Node node1 in node.GetChildren())
                {
                    SetupBlackboard(node1);
                }
            }
        }
        public void TickTree()
        {
            UpdateBlackboard();
            _root.Evaluate();
        }
    }
}
