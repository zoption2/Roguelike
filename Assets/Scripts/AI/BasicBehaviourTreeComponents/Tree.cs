using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class Tree
    {
        protected Node _root;

        public void InitTree()
        {
            _root = SetupRootNode();
        }
        protected abstract Node SetupRootNode();

        public void StartTree()
        {
            _root.Evaluate();
        }
    }
}
